import { store } from "./store.js";

export function requireAuth() {
  if (!store.getUser()) {
    window.location.hash = "/login";
    return false;
  }
  return true;
}

export function requireRole(role) {
  const user = store.getUser();
  if (!user || user.role !== role) {
    window.location.hash = "/login";
    return false;
  }
  return true;
}