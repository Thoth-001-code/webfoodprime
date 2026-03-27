import { api } from "./axios.js";

export const getWallet = () =>
  api("/wallet");

export const deposit = (amount) =>
  api("/wallet/deposit", "POST", { amount });

export const getHistory = () =>
  api("/wallet/transactions");