import { OpenAPI } from './generated/core/OpenAPI';

// Configure the API client
OpenAPI.BASE = 'http://localhost:5127';
OpenAPI.WITH_CREDENTIALS = false;
OpenAPI.CREDENTIALS = 'include';

// Token resolver function
OpenAPI.TOKEN = async () => {
  if (process.client) {
    return localStorage.getItem('auth_token') || '';
  }
  return '';
};

// Headers resolver for additional configuration
OpenAPI.HEADERS = async () => {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };

  if (process.client) {
    const token = localStorage.getItem('auth_token');
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
  }

  return headers;
};

// Export all services
export * from './generated/services/ArticlesService';
export * from './generated/services/CollectionsService';
export * from './generated/services/CommentsService';
export * from './generated/services/GoodsService';
export * from './generated/services/MessagesService';
export * from './generated/services/RelationshipService';
export * from './generated/services/SeedingService';
export * from './generated/services/StaticService';
export * from './generated/services/TransactionsService';
export * from './generated/services/UserService';
export * from './generated/services/UsersService';

// Export admin service
export * from './admin';

// Export all models and types
export * from './generated';

// Export API configuration
export { OpenAPI } from './generated/core/OpenAPI';