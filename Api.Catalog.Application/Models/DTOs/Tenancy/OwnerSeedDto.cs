using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Catalog.Application.Models;

public record OwnerSeedDto
(
    string Name,
    string Email,
    string Login,
    string Password
);
