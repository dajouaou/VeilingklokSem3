import { useContext } from "react";
import Navbar from "../../shared/components/Navbar";
import HeroBanner from "../../shared/components/HeroBanner";
import MarketSnapshot from "../../shared/components/MarketSnapshot";
import FeatureHighlights from "../../shared/components/FeatureHighlights";
import ReviewSection from "../../shared/components/ReviewSection";
import CTASection from "../../shared/components/CTASection";
import Footer from "../../shared/components/Footer";
import { AuthContext } from "../auth/AuthContext";

export default function HomePage() {
    // Haal login info op
    const { token, role } = useContext(AuthContext);

    // Check login status
    const isLoggedIn = !!token;
    const isKoper = isLoggedIn && role === "Koper";

    return (
        <>
            <Navbar />

            <HeroBanner isLoggedIn={isLoggedIn} isKoper={isKoper} />

            {/* Alleen zichtbaar voor ingelogde kopers */}
            {isKoper && <MarketSnapshot />}

            <FeatureHighlights />
            <ReviewSection />

            <CTASection isLoggedIn={isLoggedIn} isKoper={isKoper} />

            <Footer />
        </>
    );
}
