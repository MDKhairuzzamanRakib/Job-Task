import React, { useState } from "react";
import api from "../api";

const UserForm = ({ onUserCreated }) => {
  const [formData, setFormData] = useState({ name: "", age: "", email: "" });
  const [message, setMessage] = useState("");

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const res = await api.post("/create-user", formData);
      setMessage("✅ User created successfully!");
      onUserCreated(); // refresh table
      setFormData({ name: "", age: "", email: "" });
    } catch (err) {
      console.error("Error creating user:", err);
      setMessage("❌ Error creating user");
    }
  };

  return (
    <div className="form-container">
      <h2>Create User</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          name="name"
          placeholder="Name"
          value={formData.name}
          onChange={handleChange}
          required
        />
        <input
          type="number"
          name="age"
          placeholder="Age"
          value={formData.age}
          onChange={handleChange}
          required
        />
        <input
          type="email"
          name="email"
          placeholder="Email"
          value={formData.email}
          onChange={handleChange}
          required
        />
        <button type="submit">Create</button>
      </form>
      {message && <p>{message}</p>}
    </div>
  );
};

export default UserForm;
