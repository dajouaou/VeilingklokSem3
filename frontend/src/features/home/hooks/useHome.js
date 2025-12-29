import { useEffect, useMemo, useState } from "react";
import { getHome } from "../api/homeApi";

function normalizeHome(dto) {
    const d = dto || {};
    const hero = d.hero || {};
    const header = d.header || {};
    const footer = d.footer || {};
    return {
        appName: d.appName || "Digitale Veilingklok",
        header: {
            rightLink: header.rightLink || { label: "Inloggen", href: "/login" }
        },
        hero: {
            kicker: hero.kicker || "Beveiligde toegang",
            title: hero.title || "Digitale veilingomgeving voor de sierteelt",
            subtitle: hero.subtitle || "Realtime, betrouwbaar en professioneel",
            primaryAction: hero.primaryAction || { label: "Inloggen", href: "/login" },
            secondaryAction: hero.secondaryAction || null
        },
        roles: Array.isArray(d.roles) ? d.roles : [],
        footer: {
            securityNote: footer.securityNote || "Beveiligde verbinding",
            copyright: footer.copyright || "© Digitale Veilingklok"
        }
    };
}

export function useHome() {
    const [raw, setRaw] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const controller = new AbortController();

        (async () => {
            try {
                setLoading(true);
                setError("");
                const dto = await getHome({ signal: controller.signal });
                setRaw(dto);
            } catch (e) {
                if (controller.signal.aborted) return;
                setError(String(e?.message ?? e));
            } finally {
                if (!controller.signal.aborted) setLoading(false);
            }
        })();

        return () => controller.abort();
    }, []);

    const data = useMemo(() => normalizeHome(raw), [raw]);

    return { data, loading, error };
}
