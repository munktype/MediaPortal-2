﻿using MediaPortal.Backend.Services.MediaLibrary;
using MediaPortal.Backend.Services.SystemResolver;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.Messaging;
using MediaPortal.Common.Services.Messaging;
using MediaPortal.Common.Services.Settings;
using MediaPortal.Common.Settings;
using MediaPortal.Common.SystemResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test.Backend
{
    class SingleTestMIA
    {
        public Guid ASPECT_ID;

        public MediaItemAspectMetadata.SingleAttributeSpecification ATTR_STRING;
        public MediaItemAspectMetadata.SingleAttributeSpecification ATTR_INTEGER;

        public SingleMediaItemAspectMetadata Metadata;
    }

    class MultipleTestMIA
    {
        public Guid ASPECT_ID;

        public MediaItemAspectMetadata.MultipleAttributeSpecification ATTR_STRING;
        public MediaItemAspectMetadata.MultipleAttributeSpecification ATTR_INTEGER;

        public MultipleMediaItemAspectMetadata Metadata;
    }

    class TestML : MediaLibrary
    {
        public TestML() : base()
        {
            _miaManagement = MIAUtils.Management;
        }
    }

    class TestMessageBroker : MessageBroker
    {
        public void Shutdown()
        {
            Dispose();
            Thread.Sleep(GC_INTERVAL + TimeSpan.FromSeconds(1));
        }
    }

    class MIAUtils
    {
        private static MIA_Management MANAGEMENT;
        private static TestML LIBRARY;
        private static TestMessageBroker BROKER;

        private static ILogger logger = null;

        public static MIA_Management Management
        {
            get { return MANAGEMENT; }
        }

        public static MediaLibrary Library
        {
            get { return LIBRARY; }
        }

        public static void Setup(bool initLibrary = false)
        {
            Base.Setup();

            logger = ServiceRegistration.Get<ILogger>();

            logger.Debug("Registering ISettingsManager service");
            ServiceRegistration.Set<ISettingsManager>(new SettingsManager());

            logger.Debug("Registering ISystemResolver service");
            ServiceRegistration.Set<ISystemResolver>(new SystemResolver());

            MANAGEMENT = new MIA_Management();

            if (initLibrary)
            {
                logger.Debug("Registering IMessageBroker service");
                ServiceRegistration.Set<IMessageBroker>(BROKER = new TestMessageBroker());

                LIBRARY = new TestML();
            }
        }

        public static void Shutdown()
        {
            BROKER.Shutdown();
        }

        public static SingleTestMIA CreateSingleMIA(string table, Cardinality cardinality, bool createStringAttribute, bool createIntegerAttribute)
        {
            SingleTestMIA mia = new SingleTestMIA();

            mia.ASPECT_ID = Guid.NewGuid();

            IList<MediaItemAspectMetadata.SingleAttributeSpecification> attributes = new List<MediaItemAspectMetadata.SingleAttributeSpecification>();
            if (createStringAttribute)
                attributes.Add(mia.ATTR_STRING = MediaItemAspectMetadata.CreateSingleStringAttributeSpecification("ATTR_STRING", 10, cardinality, false));
            if (createIntegerAttribute)
                attributes.Add(mia.ATTR_INTEGER = MediaItemAspectMetadata.CreateSingleAttributeSpecification("ATTR_INTEGER", typeof(Int32), cardinality, true));

            mia.Metadata = new SingleMediaItemAspectMetadata(mia.ASPECT_ID, table, attributes.ToArray());

            AddMediaItemAspectStorage(mia.Metadata);

            return mia;
        }

        public static MultipleTestMIA CreateMultipleMIA(string table, Cardinality cardinality, bool createStringAttribute, bool createIntegerAttribute)
        {
            MultipleTestMIA mia = new MultipleTestMIA();

            mia.ASPECT_ID = Guid.NewGuid();

            IList<MediaItemAspectMetadata.MultipleAttributeSpecification> attributes = new List<MediaItemAspectMetadata.MultipleAttributeSpecification>();
            if (createStringAttribute)
                attributes.Add(mia.ATTR_STRING = MediaItemAspectMetadata.CreateMultipleStringAttributeSpecification("ATTR_STRING", 10, cardinality, false));
            if (createIntegerAttribute)
                attributes.Add(mia.ATTR_INTEGER = MediaItemAspectMetadata.CreateMultipleAttributeSpecification("ATTR_INTEGER", typeof(Int32), cardinality, true));

            mia.Metadata = new MultipleMediaItemAspectMetadata(mia.ASPECT_ID, table, attributes.ToArray());

            AddMediaItemAspectStorage(mia.Metadata);

            return mia;
        }

        public static void AddMediaItemAspectStorage(MediaItemAspectMetadata meta)
        {
            MANAGEMENT.AddMediaItemAspectStorage(meta);
        }
    }
}