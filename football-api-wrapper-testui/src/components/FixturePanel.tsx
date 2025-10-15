import React from 'react';
import { Card, Alert } from 'react-bootstrap';
import { ApiConfiguration } from '../types/ApiConfiguration';

interface FixturePanelProps {
  config: ApiConfiguration;
}

export default function FixturePanel({ config }: FixturePanelProps) {
  return (
    <Card>
      <Card.Header>
        <h5 className="mb-0">Fixture Search</h5>
      </Card.Header>
      <Card.Body>
        <Alert variant="info">
          <strong>Fixture Panel</strong><br />
          This panel will allow you to search for fixtures and matches by various criteria including:
          <ul className="mt-2 mb-0">
            <li>Fixture ID, date ranges, or live matches</li>
            <li>League, season, and team filters</li>
            <li>Head-to-head matchups between teams</li>
            <li>Fixture statistics, events, lineups, and player stats</li>
          </ul>
        </Alert>
        <p className="text-muted">
          This is a placeholder - the full implementation would include forms to test all fixture-related API endpoints
          from the FootballAPIWrapper C# library.
        </p>
      </Card.Body>
    </Card>
  );
}