import { store } from "../utils/store.js";

export function renderNavbar() {
  const user = store.getUser();

  let html = `<a href="#/menu">Menu</a> `;

  if (!user) {
    html += `
      <a href="#/login">Login</a>
      <a href="#/register">Register</a>
      
    `;
  } else {
    html += `
      <span>${user.email} (${user.role})</span>
      <button onclick="logout()">Logout</button>
      <span id="cart-count">🛒 0</span>

    `;
  }


  document.getElementById("navbar").innerHTML = html;
}

window.logout = function () {
  localStorage.clear();
  location.hash = "/login";
};