
import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../auth/AuthContext.jsx";
import {
    fetchAanmeldingen,
    createAanmelding,
    fetchAanvoerderStats,
} from "./api/aanvoerderApi.js";

import VeildagPicker from "./components/VeildagPicker.jsx";
import AanmeldingenBeheer from "./components/AanmeldingenBeheer.jsx";

export default function AanvoerderDashboard() {
    const { token, role } = useContext(AuthContext);

    const [items, setItems] = useState([]);
    const [stats, setStats] = useState(null);

    const [loading, setLoading] = useState(false);
    const [loadError, setLoadError] = useState("");

    const [filterDate, setFilterDate] = useState("");
    const [search, setSearch] = useState("");
    const [beheerOpen, setBeheerOpen] = useState(false);
    const highlightedDates = items.map(i => new Date(i.veildatum));


    const [form, setForm] = useState({
        soort: "",
        potmaat: "",
        steellengte: "",
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
        loadDashboardData();
    }, [token, role, filterDate]);



    async function loadDashboardData() {
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
            setLoadError(err.message);
        } finally {
            setLoading(false);
        }
    }

    function handleFormChange(e) {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
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
            potmaat: form.potmaat || null,
            steellengte: form.steellengte || null,
            hoeveelheid: Number(form.hoeveelheid),
            minimumPrijs: Number(form.minimumPrijs),
            klokLocatie: form.klokLocatie,
            veildatum: new Date(form.veildatum + "T00:00:00").toISOString(),
            fotoUrl: form.fotoUrl || null,
        };


        try {
            const created = await createAanmelding({ token, data: payload });
            setItems((prev) => [...prev, created]);

            setFormSuccess("Product succesvol aangemeld.");
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

            {stats && (
                <section className="mb-4">
                    <div className="row g-3">
                        <div className="col-md-4">
                            <div className="card shadow-sm border-0">
                                <div className="card-body">
                                    <p className="text-muted mb-1">Totaal aanmeldingen</p>
                                    <p className="fs-4 fw-bold">{stats.totaalAantalAanmeldingen}</p>
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
                                    <p className="fs-4 fw-bold">€{stats.totaleOpbrengst.toFixed(2)}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            )}

            <section className="mb-5">
                <div className="card shadow-sm border-0">
                    <div className="card-body">
                        <h2 className="h4 mb-3">Nieuw product aanmelden</h2>

                        {formError && <div className="alert alert-danger">{formError}</div>}
                        {formSuccess && <div className="alert alert-success">{formSuccess}</div>}

                        <form onSubmit={handleSubmit}>
                            <div className="row g-3">

                                <div className="col-md-6">
                                    <label className="form-label">Soort *</label>
                                    <input
                                        name="soort"
                                        type="text"
                                        className="form-control"
                                        value={form.soort}
                                        onChange={handleFormChange}
                                    />
                                </div>

                                <div className="col-md-6">
                                    <label className="form-label">Potmaat</label>
                                    <input
                                        name="potmaat"
                                        type="text"
                                        className="form-control"
                                        value={form.potmaat}
                                        onChange={handleFormChange}
                                    />
                                </div>

                                <div className="col-md-6">
                                    <label className="form-label">Steellengte</label>
                                    <input
                                        name="steellengte"
                                        type="text"
                                        className="form-control"
                                        value={form.steellengte}
                                        onChange={handleFormChange}
                                    />
                                </div>


                                <div className="col-md-4">
                                    <label className="form-label">Hoeveelheid *</label>
                                    <input
                                        name="hoeveelheid"
                                        type="number"
                                        min="1"
                                        className="form-control"
                                        value={form.hoeveelheid}
                                        onChange={handleFormChange}
                                    />
                                </div>

                                <div className="col-md-4">
                                    <label className="form-label">Minimumprijs (€) *</label>
                                    <input
                                        name="minimumPrijs"
                                        type="number"
                                        step="0.01"
                                        min="0"
                                        className="form-control"
                                        value={form.minimumPrijs}
                                        onChange={handleFormChange}
                                    />
                                </div>

                                <div className="col-md-4">
                                    <label className="form-label">Kloklocatie</label>
                                    <select
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
                                    <label className="form-label">Veildatum *</label>
                                    <VeildagPicker
                                        value={form.veildatum}
                                        onChange={(value) =>
                                            setForm((prev) => ({ ...prev, veildatum: value }))
                                        }
                                        highlightedDates={items.map(i => i.veildatum)}
                                    />
                                </div>

                                <div className="col-md-8">
                                    <label className="form-label">Foto-URL</label>
                                    <input
                                        name="fotoUrl"
                                        type="url"
                                        className="form-control"
                                        value={form.fotoUrl}
                                        onChange={handleFormChange}
                                    />
                                </div>
                            </div>

                            <div className="mt-4 d-flex justify-content-end">
                                <button type="submit" className="btn btn-success">Aanmelden</button>
                            </div>
                        </form>
                    </div>
                </div>
            </section>

            <section>
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <h2 className="h4">Mijn aanmeldingen</h2>

                    <button
                        className="btn btn-outline-primary"
                        onClick={() => setBeheerOpen(true)}
                    >
                        Aanmeldingen beheren
                    </button>
                </div>

                <div className="d-flex gap-2 mb-3">
                    <div>
                        <label className="form-label mb-1">Filter op veildatum</label>
                        <input
                            type="date"
                            className="form-control"
                            value={filterDate}
                            onChange={(e) => setFilterDate(e.target.value)}
                        />
                    </div>

                    <div>
                        <label className="form-label mb-1">Zoek op soort</label>
                        <input
                            type="search"
                            className="form-control"
                            placeholder="Bijv. Rozen"
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                        />
                    </div>
                </div>

                {loadError && <p className="text-danger">{loadError}</p>}
                {loading && <p>Laden...</p>}
                {!loading && filteredItems.length === 0 && <p>Geen aanmeldingen.</p>}

                {!loading && filteredItems.length > 0 && (
                    <div className="table-responsive">
                        <table className="table table-hover">
                            <thead className="table-light">
                                <tr>
                                    <th>Foto</th>
                                    <th>Soort</th>
                                    <th>Kenmerken</th>
                                    <th>Hoeveelheid</th>
                                    <th>Min. prijs</th>
                                    <th>Kloklocatie</th>
                                    <th>Veildatum</th>
                                    <th>Verkoop</th>
                                </tr>
                            </thead>

                            <tbody>
                                {filteredItems.map((item) => (
                                    <tr key={item.id}>
                                        <td>
                                            {item.fotoUrl ? (
                                                <img
                                                    src={item.fotoUrl}
                                                    style={{
                                                        width: "64px",
                                                        height: "64px",
                                                        objectFit: "cover",
                                                        borderRadius: "8px",
                                                    }}
                                                    alt={item.soort}
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
                                                <>
                                                    <div>€{item.verkoopPrijs?.toFixed(2)} / stuk</div>
                                                    <div className="small text-muted">
                                                        Totaal: €{item.totaleOpbrengst?.toFixed(2)}
                                                        {item.koperNaam && <> – {item.koperNaam}</>}
                                                    </div>
                                                </>
                                            ) : (
                                                <span className="badge bg-secondary">Nog niet verkocht</span>
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </section>

            {beheerOpen && (
                <AanmeldingenBeheer
                    items={filteredItems}
                    token={token}
                    onClose={() => setBeheerOpen(false)}
                    onUpdated={loadDashboardData}
                />
            )}

        </main>
    );
}
