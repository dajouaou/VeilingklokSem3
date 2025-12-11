// src/api/http.js
const API_BASE = import.meta.env.VITE_API_BASE;

async function apiGet(path) {
    const res = await fetch(`${API_BASE}${path}`, {
        credentials: "include",
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "API fout");
    }

    return res.json();
}

async function apiPost(path, body = {}) {
    const res = await fetch(`${API_BASE}${path}`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(body),
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "API fout (POST)");
    }

    try {
        return await res.json();
    } catch {
        return null;
    }
}

export { apiGet, apiPost };
