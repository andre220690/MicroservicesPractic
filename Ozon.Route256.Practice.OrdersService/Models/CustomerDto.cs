﻿using System;

namespace Ozon.Route256.Practice.GatewayService.Models;

public record CustomerDto(
    int id,
    string firstName,
    string lastName,
    string mobileNumber,
    string email,
    Address defaultAddress,
    IEnumerable<AddressDto> addresses);
