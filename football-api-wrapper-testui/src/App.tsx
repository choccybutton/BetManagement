import React, { useState } from 'react';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Navbar, Nav, Tab, Tabs } from 'react-bootstrap';
import ConfigurationPanel from './components/ConfigurationPanel';
import LeaguePanel from './components/LeaguePanel';
import TeamPanel from './components/TeamPanel';
import FixturePanel from './components/FixturePanel';
import PlayerPanel from './components/PlayerPanel';
import UsageStatsPanel from './components/UsageStatsPanel';
import { ApiConfiguration } from './types/ApiConfiguration';

function App() {
  const [config, setConfig] = useState<ApiConfiguration>({
    apiKey: '',
    baseUrl: 'https://v3.football.api-sports.io',
    rapidApiHost: 'v3.football.api-sports.io'
  });

  return (
    <div className="App">
      <Navbar bg="dark" variant="dark" className="mb-4">
        <Container>
          <Navbar.Brand>Football API Wrapper Test Interface</Navbar.Brand>
        </Container>
      </Navbar>
      
      <Container>
        <ConfigurationPanel config={config} onConfigChange={setConfig} />
        
        <Tabs defaultActiveKey="leagues" id="api-tabs" className="mb-4">
          <Tab eventKey="leagues" title="Leagues">
            <LeaguePanel config={config} />
          </Tab>
          
          <Tab eventKey="teams" title="Teams">
            <TeamPanel config={config} />
          </Tab>
          
          <Tab eventKey="fixtures" title="Fixtures">
            <FixturePanel config={config} />
          </Tab>
          
          <Tab eventKey="players" title="Players">
            <PlayerPanel config={config} />
          </Tab>
          
          <Tab eventKey="usage" title="Usage Stats">
            <UsageStatsPanel config={config} />
          </Tab>
        </Tabs>
      </Container>
    </div>
  );
}

export default App;
