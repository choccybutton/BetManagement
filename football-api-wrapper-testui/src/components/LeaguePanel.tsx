import React, { useState } from 'react';
import { Card, Form, Button, Row, Col, Table, Alert, Spinner } from 'react-bootstrap';
import { ApiConfiguration } from '../types/ApiConfiguration';
import { ApiService } from '../services/ApiService';
import { League, ApiResponse } from '../types/ApiTypes';

interface LeaguePanelProps {
  config: ApiConfiguration;
}

export default function LeaguePanel({ config }: LeaguePanelProps) {
  const [leagues, setLeagues] = useState<League[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [filters, setFilters] = useState({
    name: '',
    country: '',
    season: '',
    id: '',
    search: '',
    type: '',
    current: false
  });

  const handleSearch = async () => {
    if (!config.apiKey.trim()) {
      setError('Please configure your API key first');
      return;
    }

    setLoading(true);
    setError('');
    setLeagues([]);

    try {
      const apiService = new ApiService(config);
      
      // Build parameters object, excluding empty values
      const params: any = {};
      Object.entries(filters).forEach(([key, value]) => {
        if (key === 'current') {
          if (value === true) {
            params[key] = value;
          }
        } else if (value && value !== '') {
          if (key === 'id' || key === 'season') {
            params[key] = parseInt(value as string);
          } else {
            params[key] = value;
          }
        }
      });

      const response: ApiResponse<League> = await apiService.getLeagues(params);
      setLeagues(response.response);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (field: string, value: string | boolean) => {
    setFilters({
      ...filters,
      [field]: value
    });
  };

  return (
    <div>
      <Card className="mb-4">
        <Card.Header>
          <h5 className="mb-0">League Search</h5>
        </Card.Header>
        <Card.Body>
          <Form>
            <Row>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>League ID</Form.Label>
                  <Form.Control
                    type="number"
                    placeholder="e.g., 39 for Premier League"
                    value={filters.id}
                    onChange={(e) => handleInputChange('id', e.target.value)}
                  />
                </Form.Group>
              </Col>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>League Name</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="e.g., Premier League"
                    value={filters.name}
                    onChange={(e) => handleInputChange('name', e.target.value)}
                  />
                </Form.Group>
              </Col>
            </Row>
            
            <Row>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Country</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="e.g., England"
                    value={filters.country}
                    onChange={(e) => handleInputChange('country', e.target.value)}
                  />
                </Form.Group>
              </Col>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Season</Form.Label>
                  <Form.Control
                    type="number"
                    placeholder="e.g., 2023"
                    value={filters.season}
                    onChange={(e) => handleInputChange('season', e.target.value)}
                  />
                </Form.Group>
              </Col>
            </Row>

            <Row>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Search</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Search leagues"
                    value={filters.search}
                    onChange={(e) => handleInputChange('search', e.target.value)}
                  />
                </Form.Group>
              </Col>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Type</Form.Label>
                  <Form.Select
                    value={filters.type}
                    onChange={(e) => handleInputChange('type', e.target.value)}
                  >
                    <option value="">All Types</option>
                    <option value="league">League</option>
                    <option value="cup">Cup</option>
                  </Form.Select>
                </Form.Group>
              </Col>
            </Row>

            <Row>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Check
                    type="checkbox"
                    label="Current Season Only"
                    checked={filters.current}
                    onChange={(e) => handleInputChange('current', e.target.checked)}
                  />
                </Form.Group>
              </Col>
              <Col md={6} className="d-flex align-items-end">
                <Button 
                  variant="primary" 
                  onClick={handleSearch}
                  disabled={loading || !config.apiKey.trim()}
                  className="mb-3"
                >
                  {loading ? <><Spinner size="sm" className="me-2" />Searching...</> : 'Search Leagues'}
                </Button>
              </Col>
            </Row>
          </Form>
        </Card.Body>
      </Card>

      {error && (
        <Alert variant="danger">
          {error}
        </Alert>
      )}

      {leagues.length > 0 && (
        <Card>
          <Card.Header>
            <h5 className="mb-0">Results ({leagues.length} leagues found)</h5>
          </Card.Header>
          <Card.Body>
            <div style={{ maxHeight: '500px', overflowY: 'auto' }}>
              <Table striped bordered hover responsive>
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Logo</th>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Country</th>
                    <th>Current Season</th>
                  </tr>
                </thead>
                <tbody>
                  {leagues.map((league) => (
                    <tr key={league.id}>
                      <td>{league.id}</td>
                      <td>
                        {league.logo && (
                          <img 
                            src={league.logo} 
                            alt={league.name} 
                            style={{ width: '30px', height: '30px' }}
                          />
                        )}
                      </td>
                      <td>{league.name}</td>
                      <td>
                        <span className={`badge bg-${league.type === 'league' ? 'primary' : 'secondary'}`}>
                          {league.type}
                        </span>
                      </td>
                      <td>
                        <div className="d-flex align-items-center">
                          {league.country.flag && (
                            <img 
                              src={league.country.flag} 
                              alt={league.country.name} 
                              style={{ width: '20px', height: '15px', marginRight: '8px' }}
                            />
                          )}
                          {league.country.name}
                        </div>
                      </td>
                      <td>
                        {league.seasons?.find(s => s.current) ? (
                          <span className="badge bg-success">
                            {league.seasons.find(s => s.current)?.year}
                          </span>
                        ) : (
                          <span className="text-muted">-</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>
          </Card.Body>
        </Card>
      )}
    </div>
  );
}