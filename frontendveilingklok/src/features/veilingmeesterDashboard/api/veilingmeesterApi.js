// src/api/veilingmeesterApi.js
const BASE = import.meta.env.VITE_API_BASE;

export async function apiGet(url) {
    const r = await fetch(BASE + url, { credentials: "include" });
    if (!r.ok) throw new Error("GET " + url + " failed");
    return r.json();
}

export async function apiPost(url, body) {
    const r = await fetch(BASE + url, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
    });
    if (!r.ok) throw new Error("POST " + url + " failed");
    return r.json().catch(() => null);
}

export async function apiPut(url, body) {
    const r = await fetch(BASE + url, {
        method: "PUT",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
    });
    if (!r.ok) throw new Error("PUT " + url + " failed");
    return r.json().catch(() => null);
}
