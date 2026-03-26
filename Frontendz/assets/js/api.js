const BASE_URL = "http://localhost:7229/api";

async function api(url, method = "GET", body = null) {
    const token = localStorage.getItem("token");

    const res = await fetch(BASE_URL + url, {
        method: method,
        headers: {
            "Content-Type": "application/json",
            "Authorization": token ? "Bearer " + token : ""
        },
        body: body ? JSON.stringify(body) : null
    });

    const text = await res.text();

    if (!res.ok) {
        alert(text);
        return null;
    }

    return text ? JSON.parse(text) : null;
}