import { API_BASE_URL } from "../../../config/apiBaseUrl";

// Haalt prijshistorie op voor een bepaalde soort (en optioneel een aanvoerder)
// Wordt gebruikt voor grafieken / historisch overzicht in de UI
export async function fetchPrijsHistorie({ token, soort, aanvoerderId }) {
    // Soort omzetten naar string en opschonen (null/undefined voorkomen)
    const soortStr = String(soort ?? "").trim();

    // Basisvalidatie: soort is verplicht
    if (!soortStr) {
        throw new Error("Soort ontbreekt.");
    }

    // URL opbouwen met queryparameters
    const url = new URL(`${API_BASE_URL}/api/prijshistorie`);
    url.searchParams.set("soort", soortStr);

    // AanvoerderId is optioneel (bijv. alleen eigen historie tonen)
    if (aanvoerderId != null) {
        url.searchParams.set("aanvoerderId", String(aanvoerderId));
    }

    // Headers opbouwen
    const headers = {};

    // Token is optioneel: publieke view mag zonder login
    if (token) {
        headers.Authorization = `Bearer ${token}`;
    }

    // Request uitvoeren
    const res = await fetch(url.toString(), { headers });

    // Probeer response-body te parsen (kan leeg zijn)
    let data = null;
    try {
        data = await res.json();
    } catch {
        // Geen JSON-body, negeren
    }

    // Bij foutstatus: duidelijke foutmelding genereren
    if (!res.ok) {
        const msg =
            typeof data === "string"
                ? data
                : data?.message || data?.title || "Kon prijshistorie niet laden";

        throw new Error(msg);
    }

    // Succes: parsed data teruggeven
    return data;
}
