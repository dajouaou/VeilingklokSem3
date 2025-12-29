const API_BASE = import.meta.env.VITE_API_BASE;

function getToken() {
    return localStorage.getItem("token") || "";
}

async function request(path, options) {
    const token = getToken();
    const headers = new Headers(options?.headers || {});
    if (token) headers.set("Authorization", `Bearer ${token}`);

    const res = await fetch(`${API_BASE}${path}`, { ...options, headers });
    const text = await res.text();

    if (!res.ok) throw new Error(text || `HTTP ${res.status}`);

    if (!text) return null;

    try {
        return JSON.parse(text);
    } catch {
        return text;
    }
}

const base = "/api/veilingmeester/veilingen";

export const VeilingmeesterApi = {
    getActieveVeiling: () => request(`${base}/actief`, { method: "GET" }),

    getDashboard: (veilingId) =>
        request(`${base}/${veilingId}/dashboard`, { method: "GET" }),

    startVeiling: (veilingId) =>
        request(`${base}/${veilingId}/start`, { method: "POST" }),

    pauseVeiling: (veilingId) =>
        request(`${base}/${veilingId}/pause`, { method: "POST" }),

    resumeVeiling: (veilingId) =>
        request(`${base}/${veilingId}/resume`, { method: "POST" }),

    stopVeiling: (veilingId) =>
        request(`${base}/${veilingId}/stop`, { method: "POST" }),

    activateNext: (veilingId) =>
        request(`${base}/${veilingId}/next`, { method: "POST" }),

    closeCurrent: (veilingId) =>
        request(`${base}/${veilingId}/close-current`, { method: "POST" }),

    resetVeiling: (veilingId) =>
        request(`${base}/${veilingId}/reset`, { method: "POST" }),

    reorderQueue: (veilingId, orderedVeilingProductIds) =>
        request(`${base}/${veilingId}/queue/reorder`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ orderedVeilingProductIds }),
        }),

    skipProduct: (veilingId, veilingProductId) =>
        request(`${base}/${veilingId}/queue/skip/${veilingProductId}`, {
            method: "POST",
        }),
};
