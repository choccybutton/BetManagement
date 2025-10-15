import React, { useState } from 'react';
import { Card, Form, Button, Alert, Badge } from 'react-bootstrap';
import { ApiConfiguration } from '../types/ApiConfiguration';
import { ApiService } from '../services/ApiService';

interface ConfigurationPanelProps {
  config: ApiConfiguration;
  onConfigChange: (config: ApiConfiguration) => void;
}

export default function ConfigurationPanel({ config, onConfigChange }: ConfigurationPanelProps) {
  const [testResult, setTestResult] = useState<{ success: boolean; message: string } | null>(null);
  const [testing, setTesting] = useState(false);

  const handleInputChange = (field: keyof ApiConfiguration, value: string) => {
    onConfigChange({
      ...config,
      [field]: value
    });
  };

  const handleTestConnection = async () => {
    if (!config.apiKey.trim()) {
      setTestResult({ success: false, message: 'API key is required' });
      return;
    }

    setTesting(true);
    setTestResult(null);

    try {
      const apiService = new ApiService(config);
      const success = await apiService.testConnection();
      setTestResult({
        success,
        message: success ? 'Connection successful!' : 'Connection failed - please check your API key and configuration'
      });
    } catch (error: any) {
      setTestResult({
        success: false,
        message: `Connection failed: ${error.message}`
      });
    } finally {
      setTesting(false);
    }
  };

  return (
    <Card className="mb-4">
      <Card.Header className="d-flex justify-content-between align-items-center">
        <h5 className="mb-0">API Configuration</h5>
        {config.apiKey && (
          <Badge bg={testResult?.success ? 'success' : 'secondary'}>
            {testResult?.success ? 'Connected' : 'Not Tested'}
          </Badge>
        )}
      </Card.Header>
      <Card.Body>
        <Form>
          <Form.Group className="mb-3">
            <Form.Label>API Key *</Form.Label>
            <Form.Control
              type="password"
              placeholder="Enter your RapidAPI key"
              value={config.apiKey}
              onChange={(e) => handleInputChange('apiKey', e.target.value)}
            />
            <Form.Text className="text-muted">
              Get your API key from <a href="https://rapidapi.com/api-sports/api/api-football/" target="_blank" rel="noopener noreferrer">RapidAPI</a>
            </Form.Text>
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Base URL</Form.Label>
            <Form.Control
              type="url"
              value={config.baseUrl}
              onChange={(e) => handleInputChange('baseUrl', e.target.value)}
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>RapidAPI Host</Form.Label>
            <Form.Control
              type="text"
              value={config.rapidApiHost}
              onChange={(e) => handleInputChange('rapidApiHost', e.target.value)}
            />
          </Form.Group>

          <div className="d-flex justify-content-between align-items-center">
            <Button 
              variant="primary" 
              onClick={handleTestConnection}
              disabled={testing || !config.apiKey.trim()}
            >
              {testing ? 'Testing...' : 'Test Connection'}
            </Button>

            {testResult && (
              <Alert 
                variant={testResult.success ? 'success' : 'danger'} 
                className="mb-0 py-2 px-3"
              >
                {testResult.message}
              </Alert>
            )}
          </div>
        </Form>
      </Card.Body>
    </Card>
  );
}