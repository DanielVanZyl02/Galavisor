INSERT INTO UserRole (UserRoleID, RoleName) VALUES
(1, 'Admin'),
(2, 'Traveler');

INSERT INTO Planet (PlanetID, Name, Atmosphere, Temperature, Colour) VALUES
(1, 'Neonara', 'Nitrogen-Oxygen with glowing particles', 22, 'Electric Blue'),
(2, 'Crystalia', 'Thin argon-crystal mist', -10, 'Prismatic'),
(3, 'Pyronis', 'Dense sulfur compounds', 300, 'Fiery Orange'),
(4, 'Aquarion', 'Oxygen-rich with water vapor oceans', 15, 'Deep Blue'),
(5, 'Graviton Prime', 'Stable graviton field (no atmosphere)', -100, 'Shifting Silver');

INSERT INTO Activity (ActivityID, Name) VALUES
(1, 'Neon Surfing'),
(2, 'Crystal Canyon Hiking'),
(3, 'Lava Diving'),
(4, 'Deep Sea Bioluminescence Tour'),
(5, 'Zero-G Acrobatics'),
(6, 'Alien Wildlife Safari'),
(7, 'Meteor Shower Viewing'),
(8, 'Interplanetary Cuisine Tasting'),
(9, 'Ancient Ruins Exploration'),
(10, 'Aurora Chasing');
 
-- Insert Transport Options
INSERT INTO Transport (TransportID, Name) VALUES
(1, 'Quantum Hopper'),
(2, 'Anti-Grav Sled'),
(3, 'Plasma Jet'),
(4, 'Bio-Mechanical Glider'),
(5, 'Wormhole Shuttle');
 
-- Insert Users
INSERT INTO "User" (UserID, UserRoleID, PlanetID, Name, IsActive) VALUES
(1, 1, 1, 'Zara Xenon', TRUE),
(2, 2, 3, 'Rex Solaris', TRUE),
(3, 1, 2, 'Mira Crystalia', TRUE),
(4, 2, 4, 'Kael Aquarius', TRUE),
(5, 1, 5, 'Nova Gravitas', TRUE);
 
-- Assign Activities to Planets
INSERT INTO PlanetActivity (PlanetID, ActivityID) VALUES
(1, 1), (1, 8), (1, 10),  -- Neonara activities
(2, 2), (2, 6), (2, 9),   -- Crystalia activities
(3, 3), (3, 7),           -- Pyronis activities
(4, 4), (4, 6), (4, 8),   -- Aquarion activities
(5, 5), (5, 7), (5, 10);  -- Graviton Prime activities
 
-- Assign Transport to Planets
INSERT INTO PlanetTransport (PlanetID, TransportID) VALUES
(1, 1), (1, 3),  -- Neonara transports
(2, 2), (2, 4),  -- Crystalia transports
(3, 3),          -- Pyronis transport
(4, 4), (4, 5),  -- Aquarion transports
(5, 1), (5, 5);  -- Graviton Prime transports

-- Insert Reviews
INSERT INTO Review (ReviewID, UserID, PlanetID, Comment, Rating) VALUES
(1, 2, 1, 'The neon waves were absolutely mind-blowing! Best surfing in the galaxy!', 5),
(2, 2, 2, 'Crystal canyons were beautiful but the argon mist made breathing tricky.', 3),
(3, 3, 3, 'Lava diving is NOT for the faint-hearted. 10/10 would melt again!', 5),
(4, 4, 4, 'The bioluminescent sea creatures were magical. Like swimming in stars.', 4),
(5, 5, 5, 'Zero-G acrobatics changed my perspective on... well, everything!', 5),
(6, 1, 2, 'Our guide Mira was incredible - she showed us hidden crystal formations!', 5),
(7, 3, 4, 'A bit too wet for my taste, but the food was amazing.', 3),
(8, 4, 5, 'The shifting gravity fields take some getting used to, but wow!', 4),
(9, 5, 1, 'Nightlife on Neonara is wild! The whole planet pulses with energy.', 5),
(10, 1, 3, 'Bring your heat-resistant suit! Pyronis lives up to its name.', 4);