import { API_BASE_URL } from "../../../config/apiBaseUrl";

// Algemene helper voor GET requests
// - Bouwt de volledige API-url op
// - Voegt Authorization header toe als er een token is
// - Verwerkt JSON response en foutmeldingen
async function apiGet(path, token) {
    const res = await fetch(`${API_BASE_URL}${path}`, {
        headers: token ? { Authorization: `Bearer ${token}` } : undefined,
    });

    // Probeer response om te zetten naar JSON
    let data = null;
    try {
        data = await res.json();
    } catch {
        // Response heeft geen JSON-body
    }

    // Bij foutstatus: duidelijke foutmelding geven
    if (!res.ok) {
        const message =
            typeof data === "string"
                ? data
                : data?.message || data?.title || `GET ${path} mislukt`;
        throw new Error(message);
    }

    // Succesvolle response teruggeven
    return data;
}

// Algemene helper voor POST requests
// - Verstuurt JSON body
// - Gebruikt Authorization token
// - Centrale foutafhandeling
async function apiPost(path, token, body) {
    const res = await fetch(`${API_BASE_URL}${path}`, {
        method: "POST",
        headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
        },
        body: body ? JSON.stringify(body) : null,
    });

    // Probeer response te parsen
    let data = null;
    try {
        data = await res.json();
    } catch {
        // Response bevat geen JSON
    }

    // Foutmelding afhandelen
    if (!res.ok) {
        const message =
            typeof data === "string"
                ? data
                : data?.message || data?.title || `POST ${path} mislukt`;
        throw new Error(message);
    }

    // Succesvolle response teruggeven
    return data;
}

/* Veilingmeester beheer */

// Haalt de momenteel actieve veiling op
export function getActiveVeiling(token) {
    return apiGet("/api/veilingmeester/veilingen/actief", token);
}

// Start een geplande veiling
export function startVeiling(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/start`, token);
}

// Pauzeert een lopende veiling
export function pauseVeiling(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/pause`, token);
}

// Hervat een gepauzeerde veiling
export function resumeVeiling(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/resume`, token);
}

// Stopt (afsluiten) van een veiling
export function stopVeiling(token, veilingId) {
    return apiPost(`/api/veilingmeester/veilingen/${veilingId}/stop`, token);
}

/* Planning */

// Haalt alle beschikbare veilingdagen op
export function fetchVeilingDagen(token) {
    return apiGet("/api/veilingmeester/planning/veildagen", token);
}

// Haalt aanmeldingen op voor een specifieke leverdatum
export function fetchAanmeldingenVoorDatum(token, leverdatum) {
    return apiGet(
        `/api/veilingmeester/planning/aanmeldingen?leverdatum=${encodeURIComponent(
            leverdaturmFix(leverdatum)
        )}`,
        token
    );
}

// Helperfunctie om leverdatum veilig te maken
// - voorkomt null
// - verwijdert spaties
function leverdaturmFix(leverd) {
    return (leverd ?? "").trim();
}

// Plant een nieuwe veiling
export function planVeiling(token, body) {
    return apiPost("/api/veilingmeester/planning/plan", token, body);
}

// Haalt alle geplande veilingen op
export function fetchPlannedVeilingen(token) {
    return apiGet("/api/veilingmeester/planning/gepland", token);
}

// Haalt de eerstvolgende geplande veiling op
export function fetchVolgendeVeiling(token) {
    return apiGet("/api/veilingmeester/planning/volgende", token);
}

/* Archief */

// Haalt afgesloten veilingen (archief) op
export function fetchArchiefVeilingen(token) {
    return apiGet("/api/veilingmeester/veilingen/archief", token);
}
