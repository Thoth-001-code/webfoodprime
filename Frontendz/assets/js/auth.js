async function login() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    const res = await api("/auth/login", "POST", {
        Username: username,
        Password: password
    });

    if (!res) return;

    localStorage.setItem("token", res.token);
    showToast("Đăng nhập thành công");

    renderAuth();
    navigate("menu");
}

async function register() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    const res = await api("/auth/register", "POST", {
        Username: username,
        Password: password
    });

    if (!res) return;

    showToast("Đăng ký thành công");
    navigate("login");
}

function logout() {
    localStorage.removeItem("token");
    renderAuth();
    navigate("menu");
}