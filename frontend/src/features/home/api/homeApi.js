const API_BASE = import.meta.env.VITE_API_BASE;

export async function getHome({ signal } = {}) {
    const res = await fetch(`${API_BASE}/api/home`, {
        method: "GET",
        headers: { Accept: "application/json" },
        signal
    });
    if (!res.ok) throw new Error(await res.text());
    return res.json();
}
