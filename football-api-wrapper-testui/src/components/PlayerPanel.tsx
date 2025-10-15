import React from 'react';
import { Card, Alert } from 'react-bootstrap';
import { ApiConfiguration } from '../types/ApiConfiguration';

interface PlayerPanelProps {
  config: ApiConfiguration;
}

export default function PlayerPanel({ config }: PlayerPanelProps) {
  return (
    <Card>
      <Card.Header>
        <h5 className="mb-0">Player Search</h5>
      </Card.Header>
      <Card.Body>
        <Alert variant="info">
          <strong>Player Panel</strong><br />
          This panel will allow you to search for players and their statistics by various criteria including:
          <ul className="mt-2 mb-0">
            <li>Player ID, name, or search term</li>
            <li>Team, league, and season filters</li>
            <li>Top scorers, assists, and cards</li>
            <li>Player statistics and career information</li>
          </ul>
        </Alert>
        <p className="text-muted">
          This is a placeholder - the full implementation would include forms to test all player-related API endpoints
          from the FootballAPIWrapper C# library.
        </p>
      </Card.Body>
    </Card>
  );
}