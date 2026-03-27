import { api } from "./axios.js";

export const getCart = () =>
  api("/cart");

export const addToCart = (foodId, quantity = 1) =>
  api("/cart/add", "POST", { foodId, quantity });

export const removeFromCart = (foodId) =>
  api(`/cart/remove/${foodId}`, "DELETE");