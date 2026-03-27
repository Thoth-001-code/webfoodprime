import { requireAuth } from "./utils/auth.js";
import { renderNavbar } from "./components/navbar.js";

const routes = {
  "/login": "/pages/auth/login.html",
  "/register": "/pages/auth/register.html",
  "/menu": "/pages/user/menu.html",
    "/cart": "/pages/user/cart.html",
    "/orders": "/pages/user/orders.html",
"/wallet": "/pages/user/wallet.html"
};

function loadPage(path) {
  const page = routes[path];
  if (!page) return;

  fetch(page)
    .then(res => res.text())
    .then(html => {
      document.getElementById("app").innerHTML = html;

      runScript(path);
      renderNavbar();

      // 🔥 CALL PAGE FUNCTION
      setTimeout(() => {
         if (path === "/menu") window.loadFoods();
          if (path === "/cart") window.loadCart();
          if (path === "/orders") window.loadOrders();
          if (path === "/wallet") window.loadWallet();
      }, 100);
    });
}

function runScript(path) {
  if (path === "/login" || path === "/register") {
    import("./scripts/auth.js");
  }

    if (path === "/menu" || path === "/cart") {
    import("./scripts/user.js");
  }
  
  if (path === "/orders" || path === "/wallet") {
  import("./scripts/user.js");
}
}

// ROUTER
window.addEventListener("hashchange", router);
window.addEventListener("load", router);

function router() {
  let path = location.hash.replace("#", "") || "/login";

  if (!["/login", "/register"].includes(path)) {
    if (!requireAuth()) return;
  }

  loadPage(path);
}