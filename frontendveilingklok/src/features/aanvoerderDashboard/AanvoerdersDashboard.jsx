import React, { useEffect, useState, useContext } from "react";
import { getAanmeldingen } from "./api/aanmeldingenApi";
import { AuthContext } from "../../auth/AuthContext";
import Header from "../../components/Header";
import Footer from "../../components/Footer";
import { useNavigate } from "react-router-dom";

export default function AanvoerdersDashboard() {
    const { token, role } = useContext(AuthContext);
    const [aanmeldingen, setAanmeldingen] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        if (!token) {
            navigate("/login");
            return;
        }
        if (role !== "Aanvoerder") {
            navigate("/");
            return;
        }

        async function load() {
            try {
                const data = await getAanmeldingen(token);
                setAanmeldingen(data);
            } catch (err) {
                console.error(err);
            }
        }

        load();

    }, [token, role, navigate]);

    return (
        <>
            <Header />
            <main>
                <h2>Mijn Aangemelde Producten</h2>

                {aanmeldingen.length === 0 ? (
                    <p>Geen aanmeldingen gevonden.</p>
                ) : (
                    <ul className="aanmeldingen-lijst">
                        {aanmeldingen.map((a) => (
                            <li key={a.id}>
                                <img src={a.fotoUrl} alt={`Foto van ${a.productNaam}`} width="140" />
                                <h3>{a.productNaam}</h3>
                                <p>Soort: {a.soort}</p>
                                <p>Aantal: {a.aantal}</p>
                                <p>Minimumprijs: €{a.minimumPrijs}</p>
                                <p>Locatie: {a.locatie}</p>
                                <p>Veilingdatum: {a.veilingDatum}</p>
                            </li>
                        ))}
                    </ul>
                )}
            </main>
            <Footer />
        </>
    );
}
