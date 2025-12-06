const API_BASE = import.meta.env.VITE_API_BASE || "https://localhost:7140";

export async function getDashboard(veilingId) {
    const res = await fetch(`${API_BASE}/api/veilingen/${veilingId}/vm/dashboard`);
    return await res.json();
}

export async function startVeiling(veilingId) {
    const res = await fetch(`${API_BASE}/api/veilingen/${veilingId}/vm/start`, {
        method: "POST",
    });
    return await res.json();
}

export async function nextProduct(veilingId) {
    const res = await fetch(`${API_BASE}/api/veilingen/${veilingId}/vm/next`, {
        method: "POST",
    });
    return await res.json();
}

export async function closeCurrent(veilingId) {
    const res = await fetch(`${API_BASE}/api/veilingen/${veilingId}/vm/close-current`, {
        method: "POST",
    });
    return await res.json();
}
