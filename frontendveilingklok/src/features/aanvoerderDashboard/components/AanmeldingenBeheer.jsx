import React, { useRef, useState } from "react";
import { updateAanmelding, deleteAanmelding } from "../api/aanvoerderApi";

export default function AanmeldingenBeheer({ items, onClose, token, onUpdated }) {
    const [editingId, setEditingId] = useState(null);
    const [confirmDeleteId, setConfirmDeleteId] = useState(null);
    const [beschrijvingItem, setBeschrijvingItem] = useState(null);

    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const fotoInputRef = useRef(null);

    const [form, setForm] = useState({
        soort: "",
        potmaat: "",
        steellengte: "",
        hoeveelheid: "",
        minimumPrijs: "",
        klokLocatie: "Naaldwijk",
        leverdatum: "", // ✅ FIX
        fotoFile: null,
        beschrijving: "",
    });

    function clearMessages() {
        if (error) setError("");
        if (success) setSuccess("");
    }

    function toDateInputValue(dateLike) {
        if (!dateLike) return "";
        const d = new Date(dateLike);
        if (Number.isNaN(d.getTime())) return "";
        return d.toISOString().split("T")[0];
    }

    function startEdit(item) {
        setEditing(item.id);

        const ld = (item.leverDatum ?? item.leverdatum ?? "").toString();
        const leverdatumYmd = ld.includes("T") ? ld.split("T")[0] : ld;

        setForm({
            soort: item.soort,
            potmaat: item.potmaat || "",
            steellengte: item.steellengte || "",
            hoeveelheid: item.hoeveelheid,
            minimumPrijs: item.minimumPrijs,
            klokLocatie: item.klokLocatie,
            leverdatum: leverdatumYmd, // ✅ FIX
            beschrijving: item.beschrijving || "",
            fotoFile: null,
            beschrijving: item.beschrijving ?? "",
        });

        if (fotoInputRef.current) fotoInputRef.current.value = "";
    }

    function cancelEdit() {
        setEditingId(null);
        clearMessages();
        if (fotoInputRef.current) fotoInputRef.current.value = "";
    }

    function handleChange(e) {
        const { name, value } = e.target;
        clearMessages();
        setForm((prev) => ({ ...prev, [name]: value }));
    }

    async function saveEdit() {
        clearMessages();

        if (!editingId) return;

        if (!form.soort || !form.hoeveelheid || !form.minimumPrijs || !form.leverdatum) {
            setError("Vul minimaal soort, hoeveelheid, minimumprijs en leverdatum in.");
            return;
        }

        // yyyy-mm-dd -> ISO (consistent met backend DateTime)
        const leverdatumIso = new Date(`${form.leverdatum}T00:00:00`).toISOString();

        try {
            await updateAanmelding({
                token,
                id: editingId,
                data: {
                    soort: form.soort,
                    potmaat: form.potmaat || null,
                    steellengte: form.steellengte || null,
                    hoeveelheid: Number(form.hoeveelheid),
                    minimumPrijs: Number(form.minimumPrijs),
                    klokLocatie: form.klokLocatie,
                    // FIX: juiste key
                    leverdatum: leverdatumIso,
                    fotoFile: form.fotoFile || null,
                    beschrijving: form.beschrijving || "",
                },
            });

            setSuccess("Aanmelding succesvol bijgewerkt.");

            if (fotoInputRef.current) fotoInputRef.current.value = "";

            await onUpdated();
            setEditingId(null);
        } catch (err) {
            setError(err?.message || "Opslaan mislukt.");
        }
    }

    function handleFormChange(e) {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    }

    return (
        <div className="modal d-block" tabIndex="-1" style={{ background: "rgba(0,0,0,0.5)" }}>
            <div className="modal-dialog modal-xl">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">Aanmeldingen beheren</h5>
                        <button className="btn-close" onClick={onClose}></button>
                    </div>

                    <div className="modal-body">
                        <table className="table table-hover">
                            <thead>
                                <tr>
                                    <th>Soort</th>
                                    <th>Hoeveelheid</th>
                                    <th>Min. prijs</th>
                                    <th>Beschrijving</th>
                                    <th>Acties</th>
                                </tr>
                            </thead>

                            <tbody>
                                {items.map((a) => (
                                    <tr key={a.id}>
                                        <td>{a.soort}</td>
                                        <td>{a.hoeveelheid}</td>
                                        <td>{a.minimumPrijs.toFixed(2)} EUR</td>

                                            <td>
                                                {item.beschrijving ? (
                                                    <button
                                                        className="btn btn-sm btn-outline-secondary"
                                                        onClick={() => setBeschrijvingItem(item)}
                                                    >
                                                        Bekijken
                                                    </button>
                                                ) : (
                                                    <span className="text-muted">-</span>
                                                )}
                                            </td>

                                        <td>
                                            <button className="btn btn-sm btn-primary me-2" onClick={() => startEdit(a)}>
                                                Bewerken
                                            </button>

                                            <button className="btn btn-sm btn-danger" onClick={() => setConfirmDelete(a.id)}>
                                                Verwijderen
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>

                        {editingId && (
                            <div className="mt-4 p-3 border rounded bg-light">
                                <h5>Aanmelding bewerken</h5>

                                <div className="row g-2 mt-2">
                                    <div className="col-md-4">
                                        <label className="form-label">Soort</label>
                                        <input name="soort" className="form-control" value={form.soort} onChange={handleFormChange} />
                                    </div>

                                    <div className="col-md-4">
                                        <label className="form-label">Potmaat</label>
                                        <input name="potmaat" className="form-control" value={form.potmaat} onChange={handleFormChange} />
                                    </div>

                                    <div className="col-md-4">
                                        <label className="form-label">Steellengte</label>
                                        <input
                                            name="steellengte"
                                            className="form-control"
                                            value={form.steellengte}
                                            onChange={handleChange}
                                        />
                                    </div>

                                    <div className="col-md-3">
                                        <label className="form-label">Hoeveelheid</label>
                                        <input
                                            name="hoeveelheid"
                                            type="number"
                                            min="1"
                                            className="form-control"
                                            value={form.hoeveelheid}
                                            onChange={handleChange}
                                        />
                                    </div>

                                    <div className="col-md-3">
                                        <label className="form-label">Minimumprijs (EUR)</label>
                                        <input
                                            name="minimumPrijs"
                                            type="number"
                                            step="0.01"
                                            min="0"
                                            className="form-control"
                                            value={form.minimumPrijs}
                                            onChange={handleChange}
                                        />
                                    </div>

                                    <div className="col-md-6">
                                        <label className="form-label">Leverdatum</label>
                                        <input
                                            name="leverdatum"
                                            type="date"
                                            className="form-control"
                                            value={form.leverdatum}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-6">
                                        <label className="form-label">Kloklocatie</label>
                                        <select
                                            name="klokLocatie"
                                            className="form-select"
                                            value={form.klokLocatie}
                                            onChange={handleChange}
                                        >
                                            <option value="Naaldwijk">Naaldwijk</option>
                                            <option value="Aalsmeer">Aalsmeer</option>
                                            <option value="Rijnsburg">Rijnsburg</option>
                                            <option value="Eelde">Eelde</option>
                                        </select>
                                    </div>

                                    <div className="col-12">
                                        <label className="form-label">Nieuwe foto (optioneel)</label>
                                        <input
                                            ref={fotoInputRef}
                                            type="file"
                                            accept="image/*"
                                            className="form-control"
                                            onChange={(e) => setForm((prev) => ({ ...prev, fotoFile: e.target.files?.[0] ?? null }))}
                                        />
                                    </div>

                                    <div className="col-12">
                                        <label className="form-label">Beschrijving</label>
                                        <textarea
                                            name="beschrijving"
                                            className="form-control"
                                            rows="3"
                                            value={form.beschrijving}
                                            onChange={handleChange}
                                        />
                                    </div>
                                </div>

                                <div className="d-flex justify-content-end gap-2 mt-3">
                                    <button className="btn btn-secondary" onClick={() => setEditing(null)}>
                                        Annuleren
                                    </button>

                                    <button className="btn btn-success" onClick={saveEdit}>
                                        Opslaan
                                    </button>
                                </div>
                            </div>
                        )}

                        {confirmDeleteId && (
                            <div className="alert alert-danger mt-4">
                                <h5>Weet je zeker dat je deze aanmelding wilt verwijderen?</h5>
                                <div className="d-flex justify-content-end gap-2 mt-2">
                                    <button className="btn btn-secondary" onClick={() => setConfirmDelete(null)}>
                                        Annuleren
                                    </button>

                                    <button className="btn btn-danger" onClick={confirmDeleteAction}>
                                        Verwijderen
                                    </button>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {beschrijvingItem && (
                <div className="modal d-block" tabIndex="-1" style={{ background: "rgba(0,0,0,0.5)" }}>
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Beschrijving van {beschrijvingItem.soort}</h5>
                                <button className="btn-close" onClick={() => setBeschrijvingItem(null)}></button>
                            </div>

                            <div className="modal-body">
                                <p>{beschrijvingItem.beschrijving}</p>

                                {beschrijvingItem.fotoUrl && (
                                    <img src={beschrijvingItem.fotoUrl} className="img-fluid rounded mt-3" alt="Product" />
                                )}
                            </div>

                            <div className="modal-footer">
                                <button className="btn btn-secondary" onClick={() => setBeschrijvingItem(null)}>
                                    Sluiten
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
