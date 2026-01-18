import { API_BASE_URL } from "../../../config/apiBaseUrl";

// Algemene helper voor publieke GET requests
// - Gebruikt geen Authorization (openbare endpoints)
// - Verwerkt JSON response
// - Geeft een foutmelding bij niet-succesvolle responses
async function apiGet(path) {
    const res = await fetch(`${API_BASE_URL}${path}`);

    // Probeer response om te zetten naar JSON
    let data = null;
    try {
        data = await res.json();
    } catch {
        // Response bevat geen JSON-body
    }

    // Bij foutstatus: algemene foutmelding
    if (!res.ok) {
        throw new Error(`GET ${path} mislukt`);
    }

    // Succesvolle response teruggeven
    return data;
}

// Haalt de huidige actieve veiling op voor publieke bezoekers
export function getPublicActieveVeiling() {
    return apiGet("/api/veiling-public/actief");
}

// Haalt de eerstvolgende geplande veiling op voor publieke bezoekers
export function getPublicVolgendeVeiling() {
    return apiGet("/api/veiling-public/volgende");
}
