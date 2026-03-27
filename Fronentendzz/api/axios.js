export async function api(endpoint, method = "GET", body) {
  const token = localStorage.getItem("token");

  const res = await fetch(window.CONFIG.API_URL + endpoint, {
    method,
    headers: {
      "Content-Type": "application/json",
      Authorization: token ? "Bearer " + token : ""
    },
    body: body ? JSON.stringify(body) : null
  });

  if (!res.ok) {
    const err = await res.text();
    throw new Error(err);
  }

  return res.json();
}