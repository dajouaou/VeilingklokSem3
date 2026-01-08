// /src/features/veiling/api/prijsHistorieApi.js
const API_BASE = "https://localhost:56418";

export async function fetchPrijsHistorie({ token, soort, aanvoerderId }) {
    if (!soort) throw new Error("Soort ontbreekt.");

    const url = new URL(`${API_BASE}/api/prijshistorie`);
    url.searchParams.set("soort", soort);
    if (aanvoerderId) url.searchParams.set("aanvoerderId", String(aanvoerderId));

    const res = await fetch(url.toString(), {
        headers: { Authorization: `Bearer ${token}` },
    });

    if (!res.ok) {
        let msg = "Kon prijshistorie niet laden";
        try {
            const err = await res.json();
            msg = err?.message || err?.title || msg;
        } catch { }
        throw new Error(msg);
    }

    return res.json();
}
