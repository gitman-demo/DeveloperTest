﻿ALTER TABLE [dbo].[Sizes]
    ADD CONSTRAINT [PK_Sizes] PRIMARY KEY CLUSTERED ([SizeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

