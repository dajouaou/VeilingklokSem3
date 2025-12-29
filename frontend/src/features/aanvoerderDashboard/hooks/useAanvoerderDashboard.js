import { useCallback, useEffect, useMemo, useState } from "react";
import {
    createAanmelding,
    deleteAanmelding,
    getAanmeldingen,
    getStatistieken,
    getVeildagen,
    updateAanmelding,
} from "../api/aanvoerderDashboardApi";

export function useAanvoerderDashboard({ token } = {}) {
    const [leverdatum, setLeverdatum] = useState("");
    const [zoek, setZoek] = useState("");
    const [statusFilter, setStatusFilter] = useState("all");

    const [veildagen, setVeildagen] = useState([]);
    const [stats, setStats] = useState(null);
    const [aanmeldingen, setAanmeldingen] = useState([]);
    const [selected, setSelected] = useState(null);

    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");

    const refresh = useCallback(async () => {
        setError("");
        setLoading(true);
        try {
            const [s, list] = await Promise.all([
                getStatistieken({ leverdatum }, token),
                getAanmeldingen({ leverdatum }, token),
            ]);

            setStats(s);
            setAanmeldingen(Array.isArray(list) ? list : []);

            setSelected((prev) => {
                if (!prev) return null;
                const updated = (Array.isArray(list) ? list : []).find((x) => x.id === prev.id);
                return updated ?? null;
            });
        } catch (e) {
            setError(e?.message || "Er ging iets mis.");
        } finally {
            setLoading(false);
        }
    }, [leverdatum, token]);

    const loadVeildagen = useCallback(async () => {
        try {
            const days = await getVeildagen(token);
            setVeildagen(Array.isArray(days) ? days : []);
        } catch {
            setVeildagen([]);
        }
    }, [token]);

    useEffect(() => {
        loadVeildagen();
    }, [loadVeildagen]);

    useEffect(() => {
        refresh();
    }, [refresh]);

    const derived = useMemo(() => {
        const q = (zoek || "").trim().toLowerCase();

        const filtered = aanmeldingen
            .filter((a) => {
                if (!q) return true;
                const s = `${a.soort ?? ""} ${a.beschrijving ?? ""}`.toLowerCase();
                return s.includes(q);
            })
            .filter((a) => {
                if (statusFilter === "verkocht") return a.isVerkocht === true;
                if (statusFilter === "open") return a.isVerkocht === false;
                return true;
            });

        const totaal = stats?.totaalAantalAanmeldingen ?? aanmeldingen.length;
        const verkocht = stats?.aantalVerkocht ?? aanmeldingen.filter((x) => x.isVerkocht).length;
        const opbrengst = stats?.totaleOpbrengst ?? 0;
        const nietVerkocht = Math.max(0, totaal - verkocht);

        return { filtered, totaal, verkocht, opbrengst, nietVerkocht };
    }, [aanmeldingen, stats, zoek, statusFilter]);

    const create = useCallback(
        async (dto) => {
            setError("");
            setSaving(true);
            try {
                await createAanmelding(dto, token);
                await refresh();
            } catch (e) {
                setError(e?.message || "Aanmaken mislukt.");
                throw e;
            } finally {
                setSaving(false);
            }
        },
        [refresh, token]
    );

    const update = useCallback(
        async (id, dto) => {
            setError("");
            setSaving(true);
            try {
                await updateAanmelding(id, dto, token);
                await refresh();
            } catch (e) {
                setError(e?.message || "Bewerken mislukt.");
                throw e;
            } finally {
                setSaving(false);
            }
        },
        [refresh, token]
    );

    const remove = useCallback(
        async (id) => {
            setError("");
            setSaving(true);
            try {
                await deleteAanmelding(id, token);
                setSelected((prev) => (prev?.id === id ? null : prev));
                await refresh();
            } catch (e) {
                setError(e?.message || "Verwijderen mislukt.");
                throw e;
            } finally {
                setSaving(false);
            }
        },
        [refresh, token]
    );

    return {
        loading,
        saving,
        error,

        leverdatum,
        setLeverdatum,

        veildagen,

        zoek,
        setZoek,

        statusFilter,
        setStatusFilter,

        stats,
        aanmeldingen,
        filteredAanmeldingen: derived.filtered,

        totals: {
            totaal: derived.totaal,
            verkocht: derived.verkocht,
            nietVerkocht: derived.nietVerkocht,
            opbrengst: derived.opbrengst,
        },

        selected,
        setSelected,

        refresh,
        create,
        update,
        remove,
    };
}
