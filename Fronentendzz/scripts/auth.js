import { loginApi, registerApi } from "../api/authApi.js";
import { store } from "../utils/store.js";

// LOGIN
window.login = async function () {
  try {
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    if (!email || !password) {
      alert("Nhập đầy đủ thông tin");
      return;
    }

    const res = await loginApi({ email, password });

    // 🔥 lưu token
    localStorage.setItem("token", res.token);

    // 🔥 lưu user
    store.setUser({
      email: res.email,
      role: res.role
    });

    alert("Login thành công");

    location.hash = "/menu";
  } catch (err) {
    alert(err.message);
  }
};

// REGISTER
window.register = async function () {
  try {
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const confirm = document.getElementById("confirmPassword").value;

    if (!email || !password || !confirm) {
      alert("Nhập đầy đủ thông tin");
      return;
    }

    if (password !== confirm) {
      alert("Mật khẩu không khớp");
      return;
    }

    if (password.length < 6) {
      alert("Mật khẩu >= 6 ký tự");
      return;
    }

    await registerApi({ email, password });

    alert("Đăng ký thành công");

    location.hash = "/login";
  } catch (err) {
    alert(err.message);
  }
};