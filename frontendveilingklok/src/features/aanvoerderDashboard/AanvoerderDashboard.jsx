import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../auth/AuthContext.jsx";
import {
    fetchAanmeldingen,
    createAanmelding,
    fetchAanvoerderStats,
} from "./api/aanvoerderApi.js";

export default function AanvoerderDashboard() {
    const { token, role } = useContext(AuthContext);

    const [items, setItems] = useState([]);
    const [stats, setStats] = useState(null);
    const [loading, setLoading] = useState(false);
    const [loadError, setLoadError] = useState("");

    const [filterDate, setFilterDate] = useState("");
    const [search, setSearch] = useState("");

    const [form, setForm] = useState({
        soort: "",
        potmaatOfSteellengte: "",
        hoeveelheid: "",
        minimumPrijs: "",
        klokLocatie: "Naaldwijk",
        veildatum: "",
        fotoUrl: "",
    });

    const [formError, setFormError] = useState("");
    const [formSuccess, setFormSuccess] = useState("");

    useEffect(() => {
        if (!token || role !== "Aanvoerder") return;
        loadData();
    }, [token, role, filterDate]);

    async function loadData() {
        setLoading(true);
        setLoadError("");
        try {
            const [aanmeldingen, statsDto] = await Promise.all([
                fetchAanmeldingen({ token, veildatum: filterDate || undefined }),
                fetchAanvoerderStats({ token, veildatum: filterDate || undefined }),
            ]);

            setItems(aanmeldingen);
            setStats(statsDto);
        } catch (err) {
            console.error(err);
            setLoadError(err.message);
        } finally {
            setLoading(false);
        }
    }

    function handleFormChange(e) {
        const { name, value } = e.target;
        setForm((f) => ({ ...f, [name]: value }));
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setFormError("");
        setFormSuccess("");

        if (!form.soort || !form.hoeveelheid || !form.minimumPrijs || !form.veildatum) {
            setFormError("Vul minimaal soort, hoeveelheid, minimumprijs en veildatum in.");
            return;
        }

        const payload = {
            soort: form.soort,
            potmaatOfSteellengte: form.potmaatOfSteellengte || null,
            hoeveelheid: Number(form.hoeveelheid),
            minimumPrijs: Number(form.minimumPrijs),
            klokLocatie: form.klokLocatie,
            veildatum: form.veildatum, // 'YYYY-MM-DD'
            fotoUrl: form.fotoUrl || null,
        };

        try {
            const created = await createAanmelding({ token, data: payload });
            // voeg toe of herlaad
            setItems((prev) => [...prev, created]);
            setFormSuccess("Product succesvol aangemeld voor de veiling.");
            setForm({
                soort: "",
                potmaatOfSteellengte: "",
                hoeveelheid: "",
                minimumPrijs: "",
                klokLocatie: "Naaldwijk",
                veildatum: "",
                fotoUrl: "",
            });
        } catch (err) {
            console.error(err);
            setFormError(err.message);
        }
    }

    const filteredItems = items.filter((item) => {
        if (!search) return true;
        const term = search.toLowerCase();
        return (
            item.soort.toLowerCase().includes(term) ||
            (item.potmaatOfSteellengte || "").toLowerCase().includes(term)
        );
    });

    return (
        <main id="main" className="container py-4">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h1 className="h3">Aanvoerdersdashboard</h1>
                <p className="text-muted mb-0">Beheer je veilingaanmeldingen</p>
            </div>

            {/* Statistieken */}
            {stats && (
                <section
                    aria-labelledby="stats-title"
                    className="mb-4"
                >
                    <h2 id="stats-title" className="visually-hidden">
                        Overzicht statistieken
                    </h2>
                    <div className="row g-3">
                        <div className="col-md-4">
                            <div className="card shadow-sm border-0">
                                <div className="card-body">
                                    <p className="text-muted mb-1">Totaal aanmeldingen</p>
                                    <p className="fs-4 fw-bold">
                                        {stats.totaalAantalAanmeldingen}
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card shadow-sm border-0">
                                <div className="card-body">
                                    <p className="text-muted mb-1">Verkocht</p>
                                    <p className="fs-4 fw-bold">{stats.aantalVerkocht}</p>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card shadow-sm border-0">
                                <div className="card-body">
                                    <p className="text-muted mb-1">Totale opbrengst</p>
                                    <p className="fs-4 fw-bold">
                                        €{stats.totaleOpbrengst.toFixed(2)}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            )}

            {/* Nieuwe aanmelding */}
            <section
                aria-labelledby="aanmelding-title"
                className="mb-5"
            >
                <div className="card shadow-sm border-0">
                    <div className="card-body">
                        <div className="d-flex justify-content-between align-items-center mb-3">
                            <h2 id="aanmelding-title" className="h4 mb-0">
                                Nieuw product aanmelden
                            </h2>
                        </div>

                        {formError && (
                            <div className="alert alert-danger" role="alert">
                                {formError}
                            </div>
                        )}
                        {formSuccess && (
                            <div className="alert alert-success" role="status">
                                {formSuccess}
                            </div>
                        )}

                        <form onSubmit={handleSubmit} aria-label="Formulier productaanmelding">
                            <div className="row g-3">
                                <div className="col-md-6">
                                    <label htmlFor="soort" className="form-label">
                                        Soort <span className="text-danger">*</span>
                                    </label>
                                    <input
                                        id="soort"
                                        name="soort"
                                        type="text"
                                        className="form-control"
                                        value={form.soort}
                                        onChange={handleFormChange}
                                        required
                                    />
                                </div>

                                <div className="col-md-6">
                                    <label htmlFor="potmaatOfSteellengte" className="form-label">
                                        Potmaat of steellengte
                                    </label>
                                    <input
                                        id="potmaatOfSteellengte"
                                        name="potmaatOfSteellengte"
                                        type="text"
                                        className="form-control"
                                        value={form.potmaatOfSteellengte}
                                        onChange={handleFormChange}
                                    />
                                </div>

                                <div className="col-md-4">
                                    <label htmlFor="hoeveelheid" className="form-label">
                                        Hoeveelheid (stuks) <span className="text-danger">*</span>
                                    </label>
                                    <input
                                        id="hoeveelheid"
                                        name="hoeveelheid"
                                        type="number"
                                        className="form-control"
                                        min="1"
                                        value={form.hoeveelheid}
                                        onChange={handleFormChange}
                                        required
                                    />
                                </div>

                                <div className="col-md-4">
                                    <label htmlFor="minimumPrijs" className="form-label">
                                        Minimumprijs (€) <span className="text-danger">*</span>
                                    </label>
                                    <input
                                        id="minimumPrijs"
                                        name="minimumPrijs"
                                        type="number"
                                        step="0.01"
                                        min="0"
                                        className="form-control"
                                        value={form.minimumPrijs}
                                        onChange={handleFormChange}
                                        required
                                    />
                                </div>

                                <div className="col-md-4">
                                    <label htmlFor="klokLocatie" className="form-label">
                                        Gewenste kloklocatie
                                    </label>
                                    <select
                                        id="klokLocatie"
                                        name="klokLocatie"
                                        className="form-select"
                                        value={form.klokLocatie}
                                        onChange={handleFormChange}
                                    >
                                        <option value="Naaldwijk">Naaldwijk</option>
                                        <option value="Aalsmeer">Aalsmeer</option>
                                        <option value="Rijnsburg">Rijnsburg</option>
                                        <option value="Eelde">Eelde</option>
                                    </select>
                                </div>

                                <div className="col-md-4">
                                    <label htmlFor="veildatum" className="form-label">
                                        Veildatum <span className="text-danger">*</span>
                                    </label>
                                    <input
                                        id="veildatum"
                                        name="veildatum"
                                        type="date"
                                        className="form-control"
                                        value={form.veildatum}
                                        onChange={handleFormChange}
                                        required
                                    />
                                </div>

                                <div className="col-md-8">
                                    <label htmlFor="fotoUrl" className="form-label">
                                        Foto-URL
                                    </label>
                                    <input
                                        id="fotoUrl"
                                        name="fotoUrl"
                                        type="url"
                                        className="form-control"
                                        placeholder="/images/bloemen.jpg"
                                        value={form.fotoUrl}
                                        onChange={handleFormChange}
                                    />
                                    <div className="form-text">
                                        Geef een pad of URL naar de productfoto (optioneel).
                                    </div>
                                </div>
                            </div>

                            <div className="mt-4 d-flex justify-content-end">
                                <button type="submit" className="btn btn-success rounded-pill px-4">
                                    Aanmelden
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </section>

            {/* Overzicht aanmeldingen */}
            <section
                aria-labelledby="overzicht-title"
                className="mb-5"
            >
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <h2 id="overzicht-title" className="h4 mb-0">
                        Mijn aanmeldingen
                    </h2>

                    <div className="d-flex gap-2">
                        <div>
                            <label htmlFor="filterVeildatum" className="form-label mb-1">
                                Filter op veildatum
                            </label>
                            <input
                                id="filterVeildatum"
                                type="date"
                                className="form-control"
                                value={filterDate}
                                onChange={(e) => setFilterDate(e.target.value)}
                            />
                        </div>

                        <div>
                            <label htmlFor="search" className="form-label mb-1">
                                Zoek op soort
                            </label>
                            <input
                                id="search"
                                type="search"
                                className="form-control"
                                placeholder="Bijv. Rozen"
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </div>
                    </div>
                </div>

                {loadError && (
                    <p className="text-danger" role="alert">
                        {loadError}
                    </p>
                )}

                {loading && <p className="text-muted">Laden...</p>}

                {!loading && filteredItems.length === 0 && !loadError && (
                    <p className="text-muted">Geen aanmeldingen gevonden.</p>
                )}

                {!loading && filteredItems.length > 0 && (
                    <div className="table-responsive" role="region" aria-label="Aanmeldingen tabel">
                        <table className="table table-hover align-middle">
                            <thead className="table-light">
                                <tr>
                                    <th scope="col">Foto</th>
                                    <th scope="col">Soort</th>
                                    <th scope="col">Kenmerken</th>
                                    <th scope="col">Hoeveelheid</th>
                                    <th scope="col">Min. prijs</th>
                                    <th scope="col">Kloklocatie</th>
                                    <th scope="col">Veildatum</th>
                                    <th scope="col">Verkoop</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredItems.map((item) => (
                                    <tr key={item.id}>
                                        <td>
                                            {item.fotoUrl ? (
                                                <img
                                                    src={item.fotoUrl}
                                                    alt={`Foto van ${item.soort}`}
                                                    style={{ width: "64px", height: "64px", objectFit: "cover", borderRadius: "8px" }}
                                                />
                                            ) : (
                                                <span className="text-muted">Geen foto</span>
                                            )}
                                        </td>
                                        <td>{item.soort}</td>
                                        <td>{item.potmaatOfSteellengte || "-"}</td>
                                        <td>{item.hoeveelheid}</td>
                                        <td>€{item.minimumPrijs.toFixed(2)}</td>
                                        <td>{item.klokLocatie}</td>
                                        <td>{new Date(item.veildatum).toLocaleDateString("nl-NL")}</td>
                                        <td>
                                            {item.isVerkocht ? (
                                                <div>
                                                    <div className="fw-semibold">
                                                        €{item.verkoopPrijs?.toFixed(2)} / stuk
                                                    </div>
                                                    <div className="small text-muted">
                                                        Totaal: €{item.totaleOpbrengst?.toFixed(2)}
                                                        {item.koperNaam && <> – {item.koperNaam}</>}
                                                    </div>
                                                </div>
                                            ) : (
                                                <span className="badge bg-secondary-subtle text-secondary-emphasis">
                                                    Nog niet verkocht
                                                </span>
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </section>
        </main>
    );
}
