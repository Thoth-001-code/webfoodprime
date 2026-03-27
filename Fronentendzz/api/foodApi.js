import { api } from "./axios.js";

export const getFoods = () =>
  api("/Food");