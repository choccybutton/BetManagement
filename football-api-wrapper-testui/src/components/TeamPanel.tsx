import React from 'react';
import { Card, Alert } from 'react-bootstrap';
import { ApiConfiguration } from '../types/ApiConfiguration';

interface TeamPanelProps {
  config: ApiConfiguration;
}

export default function TeamPanel({ config }: TeamPanelProps) {
  return (
    <Card>
      <Card.Header>
        <h5 className="mb-0">Team Search</h5>
      </Card.Header>
      <Card.Body>
        <Alert variant="info">
          <strong>Team Panel</strong><br />
          This panel will allow you to search for teams by various criteria including:
          <ul className="mt-2 mb-0">
            <li>Team ID, name, or search term</li>
            <li>League and season</li>
            <li>Country or venue</li>
            <li>View team statistics and information</li>
          </ul>
        </Alert>
        <p className="text-muted">
          This is a placeholder - the full implementation would include forms to test all team-related API endpoints
          from the FootballAPIWrapper C# library.
        </p>
      </Card.Body>
    </Card>
  );
}