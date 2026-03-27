import { getFoods } from "../api/foodApi.js";
import { getCart, addToCart, removeFromCart } from "../api/cartApi.js";
import { createOrder, getMyOrders } from "../api/orderApi.js";
import { getWallet, deposit, getHistory } from "../api/walletApi.js";

// 🔥 MENU
window.loadFoods = async function () {
  const container = document.getElementById("foods");

  const foods = await getFoods();

  const html = foods.map(f => `
    <div style="
      width:200px;
      border:1px solid #ccc;
      padding:10px;
      border-radius:10px;
    ">
      <h3>${f.foodName}</h3>
      <p>${f.price}</p>

      <button onclick="add(${f.foodId})">
        ➕ Thêm vào giỏ
      </button>
    </div>
  `).join("");

  container.innerHTML = html;
};

// 🔥 ADD TO CART (XỊN HƠN)
window.add = async function (foodId) {
  try {
    const btn = event.target;

    // loading UI
    btn.innerText = "Đang thêm...";
    btn.disabled = true;

    await addToCart(foodId, 1);

    // success UI
    btn.innerText = "✔ Đã thêm";
    btn.style.background = "gray";

    // reset sau 1.5s
    setTimeout(() => {
      btn.innerText = "➕ Thêm vào giỏ";
      btn.style.background = "#28a745";
      btn.disabled = false;
    }, 1500);

  } catch (err) {
    alert(err.message);
  }
};

// 🔥 CART
window.loadCart = async function () {
  const data = await getCart();

  let total = 0;

  const html = data.items.map(i => {
    const sum = i.price * i.quantity;
    total += sum;

    return `
      <div style="border:1px solid #ccc; margin:10px; padding:10px;">
        <h4>${i.foodName}</h4>
        <p>${i.price} x ${i.quantity}</p>
        <p>Thành tiền: ${sum}</p>
        <button onclick="remove(${i.foodId})">Xóa</button>
      </div>
    `;
  }).join("");

  document.getElementById("cart").innerHTML = html;
  document.getElementById("total").innerText = total;
};

// 🔥 REMOVE
window.remove = async function (foodId) {
  await removeFromCart(foodId);
  loadCart();
};

// 🔥 CHECKOUT (tạm)
window.checkout = function () {
  alert("Sang bước ORDER ở giai đoạn 3 🚀");
};




// 🔥 ĐẶT HÀNG
window.checkout = async function () {
  try {
    const paymentMethod = document.getElementById("payment").value;

    const data = {
      addressId: 1, // 🔥 tạm hardcode
      note: "",
      paymentMethod: parseInt(paymentMethod)
    };

    await createOrder(data);

    alert("🎉 Đặt hàng thành công");

    location.hash = "/orders";

  } catch (err) {
    alert(err.message);
  }
};


window.loadOrders = async function () {
  const container = document.getElementById("orders");

  container.innerHTML = "Loading...";

  try {
    let res = await getMyOrders();
    const orders = res.$values || res;

    const html = orders.map(o => `
      <div style="border:1px solid #ccc; margin:10px; padding:10px;">
        <h3>Đơn #${o.orderId}</h3>
        <p>Trạng thái: ${o.status}</p>
        <p>Tổng tiền: ${o.totalPrice}</p>

        <ul>
          ${o.items.map(i => `
            <li>${i.foodName} x ${i.quantity}</li>
          `).join("")}
        </ul>
      </div>
    `).join("");

    container.innerHTML = html;

  } catch (err) {
    container.innerHTML = err.message;
  }
};

window.loadWallet = async function () {
  try {
    const wallet = await getWallet();

    document.getElementById("balance").innerText =
      wallet.balance.toLocaleString();

    loadHistory();

  } catch (err) {
    alert(err.message);
  }
};

window.napTien = async function () {
  try {
    const amount = document.getElementById("amount").value;

    await deposit(parseInt(amount));

    alert("Nạp tiền thành công");

    loadWallet();

  } catch (err) {
    alert(err.message);
  }
};

window.loadHistory = async function () {
  const container = document.getElementById("history");

  const res = await getHistory();
  const list = res.$values || res;

  const html = list.map(t => `
    <div>
      <p>${t.description}</p>
      <p>${t.amount}</p>
    </div>
  `).join("");

  container.innerHTML = html;
};

window.updateCartCount = async function () {
  const cart = await getCart();

  const total = cart.items.reduce((sum, i) => sum + i.quantity, 0);

  const el = document.getElementById("cart-count");
  if (el) el.innerText = "🛒 " + total;
};


