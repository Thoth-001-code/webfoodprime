import { api } from "./axios.js";

export const createOrder = (data) =>
  api("/Order", "POST", data);

export const getMyOrders = () =>
  api("/Order/my");