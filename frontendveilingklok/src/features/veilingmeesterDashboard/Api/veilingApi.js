import http from "../../../core/api/http";

// ⭐ Haal alle producten op die nog niet aan een veiling gekoppeld zijn
export async function fetchBeschikbareProducten() {
    const res = await http.get("/veilingmeester/planning/producten");
    return res.data;
}

// ⭐ Start een geplande veiling
export async function startGeplandeVeiling(payload) {
    const res = await http.post("/veilingmeester/planning/start", payload);
    return res.data;
}
