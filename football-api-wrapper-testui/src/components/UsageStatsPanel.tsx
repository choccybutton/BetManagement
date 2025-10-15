import React, { useState, useEffect } from 'react';
import { Card, Row, Col, ProgressBar, Table, Button, Alert } from 'react-bootstrap';
import { ApiConfiguration } from '../types/ApiConfiguration';
import { ApiService } from '../services/ApiService';
import { UsageStatistics } from '../types/ApiTypes';

interface UsageStatsPanelProps {
  config: ApiConfiguration;
}

export default function UsageStatsPanel({ config }: UsageStatsPanelProps) {
  const [stats, setStats] = useState<UsageStatistics | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');

  const loadStats = async () => {
    if (!config.apiKey.trim()) {
      setError('Please configure your API key first');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const apiService = new ApiService(config);
      const statistics = await apiService.getUsageStatistics();
      setStats(statistics);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (config.apiKey.trim()) {
      loadStats();
    }
  }, [config.apiKey]);

  const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleString();
  };

  const formatDuration = (startDate: string) => {
    const start = new Date(startDate);
    const now = new Date();
    const diffMs = now.getTime() - start.getTime();
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    const diffDays = Math.floor(diffHours / 24);
    
    if (diffDays > 0) {
      return `${diffDays} day(s) ago`;
    } else if (diffHours > 0) {
      return `${diffHours} hour(s) ago`;
    } else {
      return 'Less than an hour ago';
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4>API Usage Statistics</h4>
        <Button 
          variant="outline-primary" 
          onClick={loadStats}
          disabled={loading || !config.apiKey.trim()}
        >
          {loading ? 'Loading...' : 'Refresh Stats'}
        </Button>
      </div>

      {error && (
        <Alert variant="danger">
          {error}
        </Alert>
      )}

      {!config.apiKey.trim() && (
        <Alert variant="warning">
          Please configure your API key to view usage statistics.
        </Alert>
      )}

      {stats && (
        <>
          <Row className="mb-4">
            <Col md={6}>
              <Card>
                <Card.Body>
                  <Card.Title>Request Limit</Card.Title>
                  <div className="mb-2">
                    <strong>{stats.requestsUsed}</strong> / {stats.requestsLimit} requests used
                  </div>
                  <ProgressBar 
                    now={(stats.requestsUsed / stats.requestsLimit) * 100}
                    variant={stats.requestsRemaining < 10 ? 'danger' : stats.requestsRemaining < 25 ? 'warning' : 'success'}
                    className="mb-2"
                  />
                  <small className="text-muted">
                    {stats.requestsRemaining} requests remaining
                  </small>
                </Card.Body>
              </Card>
            </Col>
            
            <Col md={6}>
              <Card>
                <Card.Body>
                  <Card.Title>Success Rate</Card.Title>
                  <div className="mb-2">
                    <strong>{stats.successRate.toFixed(1)}%</strong> success rate
                  </div>
                  <ProgressBar 
                    now={stats.successRate}
                    variant={stats.successRate < 80 ? 'danger' : stats.successRate < 95 ? 'warning' : 'success'}
                    className="mb-2"
                  />
                  <small className="text-muted">
                    {stats.failedRequests} failed out of {stats.totalApiCalls} total calls
                  </small>
                </Card.Body>
              </Card>
            </Col>
          </Row>

          <Card>
            <Card.Header>
              <h5 className="mb-0">Detailed Statistics</h5>
            </Card.Header>
            <Card.Body>
              <Table striped>
                <tbody>
                  <tr>
                    <td><strong>Total API Calls</strong></td>
                    <td>{stats.totalApiCalls}</td>
                  </tr>
                  <tr>
                    <td><strong>Failed Requests</strong></td>
                    <td>{stats.failedRequests}</td>
                  </tr>
                  <tr>
                    <td><strong>Average Response Time</strong></td>
                    <td>{stats.averageResponseTimeMs.toFixed(2)} ms</td>
                  </tr>
                  <tr>
                    <td><strong>Rate Limit Reset</strong></td>
                    <td>{formatDate(stats.rateLimitReset)}</td>
                  </tr>
                  <tr>
                    <td><strong>Tracking Started</strong></td>
                    <td>
                      {formatDate(stats.trackingStarted)}
                      <br />
                      <small className="text-muted">({formatDuration(stats.trackingStarted)})</small>
                    </td>
                  </tr>
                  <tr>
                    <td><strong>Last API Call</strong></td>
                    <td>{formatDate(stats.lastApiCall)}</td>
                  </tr>
                </tbody>
              </Table>
            </Card.Body>
          </Card>

          <Alert variant="info" className="mt-4">
            <strong>Note:</strong> In a production environment, these statistics would be retrieved from a backend API 
            that uses the FootballAPIWrapper C# library. The current implementation shows mock data for demonstration purposes.
          </Alert>
        </>
      )}
    </div>
  );
}