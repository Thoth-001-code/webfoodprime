let cart = JSON.parse(localStorage.getItem("cart")) || [];

// ================= MENU =================
async function loadFoods() {
    const foods = await api("/Food");

    let html = `<div class="row">`;

    foods.forEach(f => {
        html += `
        <div class="col-md-3 mb-3">
            <div class="card food-card p-2">
                <img src="${f.imagePath}" class="w-100">
                <h5>${f.foodName}</h5>
                <p>${formatPrice(f.price)}</p>
                <button class="btn btn-success" onclick="addToCart(${f.id}, '${f.foodName}', ${f.price})">
                    Thêm
                </button>
            </div>
        </div>`;
    });

    html += `</div>`;

    document.getElementById("app").innerHTML = html;
}

// ================= CART =================
function addToCart(id, name, price) {
    cart.push({ id, name, price, quantity: 1 });
    localStorage.setItem("cart", JSON.stringify(cart));
    showToast("Đã thêm vào giỏ");
}

function loadCart() {
    let html = `<h3>Giỏ hàng</h3>`;

    cart.forEach(item => {
        html += `
        <div>
            ${item.name} - ${item.quantity} x ${formatPrice(item.price)}
        </div>`;
    });

    html += `<button class="btn btn-primary mt-3" onclick="checkout()">Thanh toán</button>`;

    document.getElementById("app").innerHTML = html;
}

// ================= CHECKOUT =================
async function checkout() {
    const res = await api("/orders", "POST", {
        items: cart
    });

    if (!res) return;

    showToast("Đặt hàng thành công");
    cart = [];
    localStorage.removeItem("cart");

    navigate("orders");
}

// ================= ORDERS =================
async function loadOrders() {
    const orders = await api("/orders");

    let html = "<h3>Đơn hàng</h3>";

    orders.forEach(o => {
        html += `<div>Đơn #${o.id} - ${o.status}</div>`;
    });

    document.getElementById("app").innerHTML = html;
}