export const store = {
  user: null,

  setUser(user) {
    this.user = user;
    localStorage.setItem("user", JSON.stringify(user));
  },

  getUser() {
    return JSON.parse(localStorage.getItem("user"));
  },

  logout() {
    localStorage.removeItem("user");
    localStorage.removeItem("token");
  }
};