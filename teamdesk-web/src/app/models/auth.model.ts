export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {      // miroir de ton LoginResponse C# (en camelCase)
  token: string;
  name: string;
  role: 'User' | 'Manager';
}