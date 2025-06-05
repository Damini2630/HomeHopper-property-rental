import React from 'react';
import { FaLinkedin, FaTwitter, FaEnvelope } from 'react-icons/fa';
import './Team.css';

const teamMembers = [
  {
    id: 1,
    name: "Damini Tiwari",
    role: "Team member 1"
  },
  {
    id: 2,
    name: "Gracy Bandaru",
    role: "Team member 2"
  },
  {
    id: 3,
    name: "Aditya Achutan",
    role: "Team member 3"
  },
  {
    id: 4,
    name: "Subham Jha",
    role: "Team member 4"
  },
  {
    id: 5,
    name: "Hima Padala",
    role: "Team member 5"
  }
];

const Team = () => {
  return (
    <section className="team">
      <div className="team-header">
        <h2>Meet Our Team</h2>
        <p>The passionate people behind HomeHopper</p>
      </div>

      <div className="team-grid">
        {teamMembers.map((member) => (
          <div key={member.id} className="team-card">
            <div className="team-info">
              <h3>{member.name}</h3>
              <p className="role">{member.role}</p>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
};

export default Team;