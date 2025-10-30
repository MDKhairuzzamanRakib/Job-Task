import React, { useState } from "react";
import UserTable from "./components/UserTable";
import UserForm from "./components/UserForm";
import "./App.css";

function App() {
  const [reload, setReload] = useState(false);

  const handleUserCreated = () => {
    // trigger re-render of UserTable
    setReload((prev) => !prev);
  };

  return (
    <div className="App">
      <h1>📚 User Management Dashboard</h1>
      <UserForm onUserCreated={handleUserCreated} />
      <UserTable key={reload} />
    </div>
  );
}

export default App;
