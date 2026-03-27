import { api } from "./axios.js";

export const loginApi = (data) =>
  api("/auth/login", "POST", data);

export const registerApi = (data) =>
  api("/auth/register", "POST", data);