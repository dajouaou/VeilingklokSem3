const API_BASE = "https://localhost:56418";

async function apiGet(url, token) {
    const res = await fetch(`${API_BASE}${url}`, {
        headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error(`GET ${url} mislukt`);
    return res.json();
}

async function apiPost(url, token, body) {
    const res = await fetch(`${API_BASE}${url}`, {
        method: "POST",
        headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
        },
        body: body ? JSON.stringify(body) : null,
    });

    let json = null;
    try { json = await res.json(); } catch { }

    if (!res.ok) {
        throw new Error(json?.message || `POST ${url} mislukt`);
    }
    return json;
}

export function getActiveVeiling(token) {
    return apiGet("/api/veilingmeester/veilingen/actief", token);
}

export function startVeiling(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/start`, token);
}

export function pauseVeiling(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/pause`, token);
}

export function resumeVeiling(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/resume`, token);
}

export function stopVeiling(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/stop`, token);
}

export function placeBid(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/buy`, token);
}

export function fetchVeilingDagen(token) {
    return apiGet("/api/veilingmeester/planning/veildagen", token);
}

export function fetchAanmeldingenVoorDatum(token, leverdatum) {
    return apiGet(
        `/api/veilingmeester/planning/aanmeldingen?leverdatum=${encodeURIComponent(leverdatum)}`,
        token
    );
}

export function planVeiling(token, body) {
    return apiPost("/api/veilingmeester/planning/plan", token, body);
}

export function fetchPlannedVeilingen(token) {
    return apiGet("/api/veilingmeester/planning/gepland", token);
}
