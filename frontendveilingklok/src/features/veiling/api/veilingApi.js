import { API_BASE_URL } from "../../../config/apiBaseUrl";

async function apiGet(path, token) {
    const res = await fetch(`${API_BASE_URL}${path}`, {
        headers: token ? { Authorization: `Bearer ${token}` } : undefined,
    });

    let data = null;
    try {
        data = await res.json();
    } catch { }

    if (!res.ok) {
        const message =
            typeof data === "string"
                ? data
                : data?.message || data?.title || `GET ${path} mislukt`;
        throw new Error(message);
    }

    return data;
}

async function apiPost(path, token, body) {
    const res = await fetch(`${API_BASE_URL}${path}`, {
        method: "POST",
        headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
        },
        body: body ? JSON.stringify(body) : null,
    });

    let data = null;
    try {
        data = await res.json();
    } catch { }

    if (!res.ok) {
        const message =
            typeof data === "string"
                ? data
                : data?.message || data?.title || `POST ${path} mislukt`;
        throw new Error(message);
    }

    return data;
}

/* Veilingmeester beheer */
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

/* Planning */
export function fetchVeilingDagen(token) {
    return apiGet("/api/veilingmeester/planning/veildagen", token);
}

export function fetchAanmeldingenVoorDatum(token, leverdatum) {
    return apiGet(
        `/api/veilingmeester/planning/aanmeldingen?leverdatum=${encodeURIComponent(leverdaturmFix(leverdatum))}`,
        token
    );
}

function leverdaturmFix(leverd) {
    return (leverd ?? "").trim();
}

export function planVeiling(token, body) {
    return apiPost("/api/veilingmeester/planning/plan", token, body);
}

export function fetchPlannedVeilingen(token) {
    return apiGet("/api/veilingmeester/planning/gepland", token);
}

export function fetchVolgendeVeiling(token) {
    return apiGet("/api/veilingmeester/planning/volgende", token);
}

/* Archief */
export function fetchArchiefVeilingen(token) {
    return apiGet("/api/veilingmeester/veilingen/archief", token);
}
