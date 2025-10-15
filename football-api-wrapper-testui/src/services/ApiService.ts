import axios, { AxiosResponse } from 'axios';
import { ApiConfiguration } from '../types/ApiConfiguration';
import { ApiResponse, League, Team, FixtureDetails, PlayerStatistics, UsageStatistics } from '../types/ApiTypes';

export class ApiService {
  private baseUrl: string;
  private headers: Record<string, string>;

  constructor(config: ApiConfiguration) {
    this.baseUrl = config.baseUrl;
    this.headers = {
      'x-rapidapi-key': config.apiKey,
      'x-rapidapi-host': config.rapidApiHost,
      'Content-Type': 'application/json'
    };
  }

  private async makeRequest<T>(endpoint: string, params?: Record<string, any>): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<ApiResponse<T>> = await axios.get(
        `${this.baseUrl}/${endpoint}`,
        {
          headers: this.headers,
          params: params
        }
      );
      return response.data;
    } catch (error: any) {
      console.error('API request failed:', error);
      throw new Error(`API request failed: ${error.response?.data?.message || error.message}`);
    }
  }

  // League endpoints
  async getLeagues(params?: {
    name?: string;
    country?: string;
    code?: string;
    season?: number;
    id?: number;
    search?: string;
    type?: string;
    current?: boolean;
  }): Promise<ApiResponse<League>> {
    return this.makeRequest<League>('leagues', params);
  }

  // Team endpoints
  async getTeams(params?: {
    id?: number;
    name?: string;
    league?: number;
    season?: number;
    country?: string;
    code?: string;
    venue?: number;
    search?: string;
  }): Promise<ApiResponse<Team>> {
    return this.makeRequest<Team>('teams', params);
  }

  // Fixture endpoints
  async getFixtures(params?: {
    id?: number;
    ids?: string;
    live?: string;
    date?: string;
    league?: number;
    season?: number;
    team?: number;
    last?: number;
    next?: number;
    from?: string;
    to?: string;
    round?: string;
    status?: string;
    venue?: number;
    timezone?: string;
  }): Promise<ApiResponse<FixtureDetails>> {
    return this.makeRequest<FixtureDetails>('fixtures', params);
  }

  async getHeadToHead(h2h: string, params?: {
    date?: string;
    league?: number;
    season?: number;
    last?: number;
    next?: number;
    from?: string;
    to?: string;
    status?: string;
    venue?: number;
    timezone?: string;
  }): Promise<ApiResponse<FixtureDetails>> {
    return this.makeRequest<FixtureDetails>('fixtures/headtohead', { h2h, ...params });
  }

  // Player endpoints
  async getPlayers(params?: {
    id?: number;
    team?: number;
    league?: number;
    season?: number;
    page?: number;
    search?: string;
  }): Promise<ApiResponse<PlayerStatistics>> {
    return this.makeRequest<PlayerStatistics>('players', params);
  }

  async getTopScorers(league: number, season: number): Promise<ApiResponse<PlayerStatistics>> {
    return this.makeRequest<PlayerStatistics>('players/topscorers', { league, season });
  }

  async getTopAssists(league: number, season: number): Promise<ApiResponse<PlayerStatistics>> {
    return this.makeRequest<PlayerStatistics>('players/topassists', { league, season });
  }

  // Mock usage statistics (since we can't access C# wrapper directly from React)
  async getUsageStatistics(): Promise<UsageStatistics> {
    // In a real implementation, this would call a backend API that uses the C# wrapper
    return {
      requestsUsed: 45,
      requestsLimit: 100,
      requestsRemaining: 55,
      rateLimitReset: new Date(Date.now() + 3600000).toISOString(),
      totalApiCalls: 127,
      trackingStarted: new Date(Date.now() - 86400000).toISOString(),
      lastApiCall: new Date().toISOString(),
      averageResponseTimeMs: 245.6,
      failedRequests: 3,
      successRate: 97.6
    };
  }

  // Test connection
  async testConnection(): Promise<boolean> {
    try {
      await this.getLeagues({ id: 39 }); // Test with Premier League ID
      return true;
    } catch {
      return false;
    }
  }
}
