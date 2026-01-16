import { API_BASE_URL } from "../../../config/apiBaseUrl";

export async function fetchPrijsHistorie({ token, soort, aanvoerderId }) {
    const soortStr = String(soort ?? "").trim();
    if (!soortStr) throw new Error("Soort ontbreekt.");

    const url = new URL(`${API_BASE_URL}/api/prijshistorie`);
    url.searchParams.set("soort", soortStr);
    if (aanvoerderId != null) url.searchParams.set("aanvoerderId", String(aanvoerderId));

    const headers = {};
    if (token) headers.Authorization = `Bearer ${token}`; // token mag null zijn bij publieke view

    const res = await fetch(url.toString(), { headers });

    let data = null;
    try { data = await res.json(); } catch { }

    if (!res.ok) {
        const msg =
            typeof data === "string"
                ? data
                : data?.message || data?.title || "Kon prijshistorie niet laden";
        throw new Error(msg);
    }

    return data;
}

