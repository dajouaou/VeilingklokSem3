import Navbar from "../../shared/components/Navbar";
import HeroBanner from "../../shared/components/HeroBanner";
import MarketSnapshot from "../../shared/components/MarketSnapshot";
import FeatureHighlights from "../../shared/components/FeatureHighlights";
import ReviewSection from "../../shared/components/ReviewSection";
import CTASection from "../../shared/components/CTASection";
import Footer from "../../shared/components/Footer";

export default function HomePage() {
    return (
        <>
            <Navbar />

            <HeroBanner />
            <MarketSnapshot />
            <FeatureHighlights />
            <ReviewSection />
            <CTASection />

            <Footer />
        </>
    );
}
