// src/App.tsx
import { Routes, Route } from "react-router-dom";
import Login from "@/features/auth/pages/Login";
import Register from "@/features/auth/pages/Register";
import Home from "@/pages/Home";
import NotFound from "@/pages/NotFound";
import Navbar from "@/components/Navbar";

function App() {
  return (
    <div className="min-h-screen flex flex-col bg-orange-50">
      <Navbar />
      <main className="flex-1">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </main>
    </div>
  );
}

export default App;
