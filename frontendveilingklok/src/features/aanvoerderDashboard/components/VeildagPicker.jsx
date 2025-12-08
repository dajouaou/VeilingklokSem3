import { useState, useEffect } from "react";
import { fetchVeilDagenForAanvoerder } from "../api/aanvoerderApi";

export default function VeildagDropdown({ token, value, onChange }) {
    const [dagen, setDagen] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function load() {
            try {
                const data = await fetchVeilDagenForAanvoerder(token);
                setDagen(data);
            } catch (err) {
                console.error("Fout bij ophalen veildagen:", err);
            } finally {
                setLoading(false);
            }
        }

        load();
    }, [token]);

    if (loading) return <p>Laden...</p>;

    return (
        <select
            className="form-select"
            value={value}
            onChange={(e) => onChange(e.target.value)}
        >
            <option value="">-- Kies een veildatum --</option>
            {dagen.map((d) => (
                <option key={d} value={d}>
                    {new Date(d).toLocaleDateString("nl-NL")}
                </option>
            ))}
        </select>
    );
}
