export interface ApiResponse<T> {
  get: string;
  parameters: Record<string, any>;
  errors: string[];
  results: number;
  paging: {
    current: number;
    total: number;
  };
  response: T[];
}

export interface League {
  id: number;
  name: string;
  type: string;
  logo: string;
  country: {
    name: string;
    code: string;
    flag: string;
  };
  seasons: Season[];
}

export interface Season {
  year: number;
  start: string;
  end: string;
  current: boolean;
}

export interface Team {
  id: number;
  name: string;
  code: string;
  country: string;
  founded?: number;
  national: boolean;
  logo: string;
  venue?: {
    id?: number;
    name: string;
    address: string;
    city: string;
    capacity?: number;
    surface: string;
    image: string;
  };
}

export interface FixtureDetails {
  fixture: {
    id: number;
    referee: string;
    timezone: string;
    date: string;
    timestamp: number;
    status: {
      long: string;
      short: string;
      elapsed?: number;
    };
  };
  league: League;
  teams: {
    home: {
      id: number;
      name: string;
      logo: string;
      winner?: boolean;
    };
    away: {
      id: number;
      name: string;
      logo: string;
      winner?: boolean;
    };
  };
  goals: {
    home?: number;
    away?: number;
  };
  score: {
    halftime: { home?: number; away?: number };
    fulltime: { home?: number; away?: number };
    extratime: { home?: number; away?: number };
    penalty: { home?: number; away?: number };
  };
}

export interface PlayerStatistics {
  player: {
    id: number;
    name: string;
    firstname: string;
    lastname: string;
    age?: number;
    nationality: string;
    height: string;
    weight: string;
    photo: string;
  };
  statistics: Array<{
    team: Team;
    league: League;
    games: {
      appearences?: number;
      lineups?: number;
      minutes?: number;
      position: string;
      rating?: string;
      captain: boolean;
    };
    goals: {
      total?: number;
      assists?: number;
    };
  }>;
}

export interface UsageStatistics {
  requestsUsed: number;
  requestsLimit: number;
  requestsRemaining: number;
  rateLimitReset?: string;
  totalApiCalls: number;
  trackingStarted: string;
  lastApiCall?: string;
  averageResponseTimeMs: number;
  failedRequests: number;
  successRate: number;
}