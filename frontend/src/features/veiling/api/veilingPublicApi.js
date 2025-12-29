const API_BASE = import.meta.env.VITE_API_BASE ?? "";

function getAccessToken() {
    return null;
}

async function request(path, options = {}) {
    const headers = new Headers(options.headers || {});
    if (!headers.has("Content-Type") && options.body != null) {
        headers.set("Content-Type", "application/json");
    }

    const token = getAccessToken();
    if (token && !headers.has("Authorization")) {
        headers.set("Authorization", `Bearer ${token}`);
    }

    const res = await fetch(`${API_BASE}${path}`, {
        credentials: "include",
        ...options,
        headers,
    });

    if (res.status === 204) return null;

    const text = await res.text().catch(() => "");
    if (!res.ok) throw new Error(text || `HTTP ${res.status}`);
    if (!text) return null;

    try {
        return JSON.parse(text);
    } catch {
        return text;
    }
}

export const VeilingPublicApi = {
    getPublicLeverdagen: () => request(`/api/veilingen/public/leverdagen`),
    loadPublic: (veilingId) => request(`/api/veilingen/${veilingId}/public`),
    placeBid: (veilingId, price = null) =>
        request(`/api/veilingen/${veilingId}/bids`, {
            method: "POST",
            body: JSON.stringify({ price }),
        }),
};
